using System.Threading.Channels;
using static LocalRepositoryListing.Searcher.DirectoryUtility;

namespace LocalRepositoryListing.Searcher;

/// <summary>
/// Represents a class that searches for local repositories with specified root directories, paths to exclude, and names to exclude.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="EnumerateDirectorySearcher"/> class.
/// </remarks>
/// <param name="rootDirectories">The root directories to search in.</param>
/// <param name="excludePaths">The paths to exclude from the search.</param>
/// <param name="excludeNames">The names to exclude from the search.</param>
public class EnumerateDirectorySearcher(IList<string> rootDirectories, IList<string> excludePaths, IList<string> excludeNames) : DirectorySearcherBase(rootDirectories, excludePaths, excludeNames), ISearcher
{
    protected override EnumerationOptions EnumerationOptions { get; } = new()
    {
        RecurseSubdirectories = false,
        IgnoreInaccessible = true,
        MatchType = MatchType.Simple,
        ReturnSpecialDirectories = false,
        AttributesToSkip = FileAttributes.System
                        | FileAttributes.Compressed
                        | FileAttributes.Offline
                        | FileAttributes.Temporary
                        | FileAttributes.ReparsePoint,
    };

    private static readonly EnumerationOptions _recursiveEnumerationOptions = new()
    {
        RecurseSubdirectories = true,
        IgnoreInaccessible = true,
        MatchType = MatchType.Simple,
        ReturnSpecialDirectories = false,
        AttributesToSkip = FileAttributes.System
                        | FileAttributes.Compressed
                        | FileAttributes.Offline
                        | FileAttributes.Temporary
                        | FileAttributes.ReparsePoint,
    };

    /// <summary>
    /// Gets a value indicating whether to search for repositories recursively.
    /// </summary>
    /// <value><c>true</c> if the search is recursive; otherwise, <c>false</c>.</value>
    /// <remarks>
    /// The default value is <c>true</c>.
    /// </remarks>
    public bool RecurseSubdirectories { get; init; } = true;

    public override async Task Search(ChannelWriter<DirectoryInfo> writer, CancellationToken cancellationToken)
    {
        try
        {
            var parallelOptions = new ParallelOptions { CancellationToken = cancellationToken };
            await Parallel.ForEachAsync(RootDirectories, parallelOptions, async (rootDirectory, ct) =>
            {
                var rootEntries = Directory.EnumerateDirectories(rootDirectory, _rootSearchPattern, EnumerationOptions);

                await Parallel.ForEachAsync(rootEntries, parallelOptions, async (rootEntry, ct) =>
                {
                    var rootEntryInfo = new DirectoryInfo(rootEntry);
                    if (IsMatchExclude(rootEntryInfo, ExcludePaths, ExcludeNames))
                    {
                        return;
                    }

                    if (RecurseSubdirectories)
                    {
                        var gitDirectories = Directory.EnumerateDirectories(rootEntry, _searchPattern, _recursiveEnumerationOptions);
                        await Parallel.ForEachAsync(gitDirectories, parallelOptions, async (gitDirectory, ct) =>
                        {
                            var gitRepositoryInfo = Directory.GetParent(gitDirectory);
                            if (gitRepositoryInfo == null || IsMatchExclude(gitRepositoryInfo, ExcludePaths, ExcludeNames))
                            {
                                return;
                            }

                            await writer.WriteAsync(gitRepositoryInfo, ct);
                        });
                    }
                    else
                    {
                        var gitDirectories = rootEntryInfo.GetDirectories(_searchPattern, SearchOption.TopDirectoryOnly);
                        if (gitDirectories.Length == 0)
                        {
                            // No .git directory found in the root entry; root entry is not a git repository, so we skip it.
                            return;
                        }

                        await writer.WriteAsync(rootEntryInfo, ct);
                    }
                });
            });

            writer.Complete();
        }
        catch (OperationCanceledException e)
        {
            writer.Complete(e);
        }
    }
}
