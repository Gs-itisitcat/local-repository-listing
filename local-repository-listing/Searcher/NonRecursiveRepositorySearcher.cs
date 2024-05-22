using R3;

namespace LocalRepositoryListing.Searcher;

/// <summary>
/// Represents a class that searches for local repositories within specified root directories.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="NonRecursiveRepositorySearcher"/> class.
/// </remarks>
/// <param name="rootDirectories">The root directories to search in.</param>
/// <param name="excludePaths">The paths to exclude from the search.</param>
/// <param name="excludeNames">The names to exclude from the search.</param>
public class NonRecursiveRepositorySearcher(IList<string> rootDirectories, IList<string> excludePaths, IList<string> excludeNames) : DirectorySearcherBase(rootDirectories, excludePaths, excludeNames), ISearcher
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

    /// <summary>
    /// Initializes a new instance of the RepositorySearcher class with the specified root directories.
    /// </summary>
    /// <param name="rootDirectories">An array of root directories to search for repositories.</param>
    public NonRecursiveRepositorySearcher(IList<string> rootDirectories) : this(rootDirectories, [], []) { }

    public override Task Search(CancellationToken cancellationToken)
    {
        // Somehow cancelled or exited faster by wrapping in Task.Run
        return Task.Run(() =>
            RootDirectories
            .AsParallel()
            .WithCancellation(cancellationToken)
            .SelectMany(d => Directory.EnumerateDirectories(d, _rootSearchPattern, EnumerationOptions))
            .Select(d => new DirectoryInfo(d))
            .Where(d => d.GetDirectories(_searchPattern, SearchOption.TopDirectoryOnly).Length != 0)
            .Where(d => !IsMatchExclude(d))
            .ForAll(_searchResults.OnNext)
        , cancellationToken);

    }
}
