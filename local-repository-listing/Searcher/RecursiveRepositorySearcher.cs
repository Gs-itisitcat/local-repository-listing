namespace LocalRepositoryListing.Searcher;

/// <summary>
/// Represents a class that searches for local repositories within specified root directories.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="RecursiveRepositorySearcher"/> class.
/// </remarks>
/// <param name="rootDirectories">The root directories to search in.</param>
/// <param name="excludePaths">The paths to exclude from the search.</param>
/// <param name="excludeNames">The names to exclude from the search.</param>
public class RecursiveRepositorySearcher(IList<string> rootDirectories, IList<string> excludePaths, IList<string> excludeNames) : DirectorySearcherBase(rootDirectories, excludePaths, excludeNames), ISearcher
{
    private static readonly EnumerationOptions _rootEnumerationOptions = new()
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

    protected override EnumerationOptions EnumerationOptions { get; } = new()
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
    /// Initializes a new instance of the RepositorySearcher class with the specified root directories.
    /// </summary>
    /// <param name="rootDirectories">An array of root directories to search for repositories.</param>
    public RecursiveRepositorySearcher(IList<string> rootDirectories) : this(rootDirectories, [], []) { }

    /// <summary>
    /// Initializes a new instance of the RepositorySearcher class with the logical drives as the root directories.
    /// </summary>
    public RecursiveRepositorySearcher() : this(Environment.GetLogicalDrives()) { }

    public override ParallelQuery<DirectoryInfo> Search(CancellationToken cancellationToken)
    {
        return RootDirectories
        .AsParallel()
        .WithCancellation(cancellationToken)
        .SelectMany(d => Directory.EnumerateDirectories(d, _rootSearchPattern, _rootEnumerationOptions))
        .AsParallel() // Somehow faster with this additional AsParallel()
        .WithCancellation(cancellationToken)
        .Where(d => !IsMatchExclude(new DirectoryInfo(d)))
        .SelectMany(d => Directory.EnumerateDirectories(d, _searchPattern, EnumerationOptions))
        .Select(d => Directory.GetParent(d))
        .Where(d => d != null)
        .Select(d => d ?? throw new InvalidOperationException("Directory.GetParent returns null"))
        .Where(d => !IsMatchExclude(d));
    }
}
