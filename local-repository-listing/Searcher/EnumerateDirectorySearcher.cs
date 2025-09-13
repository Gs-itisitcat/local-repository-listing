using System.Diagnostics;
using R3;
using ZLinq;
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

    public override Task Search(CancellationToken cancellationToken)
    {
        return Task.Run(() =>
        {
            try
            {
                var insideRootDirectories = RootDirectories
                .AsParallel()
                .WithCancellation(cancellationToken)
                .SelectMany(d => Directory.EnumerateDirectories(d, _rootSearchPattern, EnumerationOptions));

                var foundDirectories = RecurseSubdirectories switch
                {
                    true => insideRootDirectories
                    .AsParallel() // Somehow faster with this additional AsParallel()
                    .WithCancellation(cancellationToken)
                    .Where(d => !IsMatchExclude(new DirectoryInfo(d), ExcludePaths, ExcludeNames))
                    .SelectMany(d => Directory.EnumerateDirectories(d, _searchPattern, _recursiveEnumerationOptions))
                    .Select(d => Directory.GetParent(d))
                    .Where(d => d != null)
                    .Select(d => d ?? throw new UnreachableException("Parent directories that are null should have already been excluded.")),

                    false => insideRootDirectories
                    .Select(d => new DirectoryInfo(d))
                    .Where(d => d.GetDirectories(_searchPattern, SearchOption.TopDirectoryOnly).Length != 0),
                };

                foundDirectories
                .Where(d => !IsMatchExclude(d, ExcludePaths, ExcludeNames))
                .ForAll(_searchSubject.OnNext);

                _searchSubject.OnCompleted(Result.Success);
            }
            catch (OperationCanceledException e)
            {
                _searchSubject.OnCompleted(Result.Failure(e));
            }
        }, cancellationToken);
    }
}
