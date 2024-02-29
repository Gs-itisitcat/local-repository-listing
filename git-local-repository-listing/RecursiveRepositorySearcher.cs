using Microsoft.Extensions.FileSystemGlobbing;
namespace ListLocalRepositories.Search;

/// <summary>
/// Represents a class that searches for local repositories within specified root directories.
/// </summary>
public class RecursiveRepositorySearcher : ISearcher
{
    private static string RootSearchPattern = "*";
    private static string SearchPattern = ".git";
    public string[] RootDirectories { get; init; }
    public string[] ExcludePaths { get; init; }
    public string[] ExcludeNames { get; init; }

    private EnumerationOptions _rootEnumerationOptions = new EnumerationOptions()
    {
        RecurseSubdirectories = false,
        IgnoreInaccessible = true,
        MatchType = MatchType.Simple,
        ReturnSpecialDirectories = false,
        AttributesToSkip = FileAttributes.System | FileAttributes.Compressed | FileAttributes.Offline | FileAttributes.Temporary | FileAttributes.ReparsePoint,
    };

    private EnumerationOptions _enumerationOptions = new EnumerationOptions()
    {
        RecurseSubdirectories = true,
        IgnoreInaccessible = true,
        MatchType = MatchType.Simple,
        ReturnSpecialDirectories = false,
        AttributesToSkip = FileAttributes.System | FileAttributes.Compressed | FileAttributes.Offline | FileAttributes.Temporary | FileAttributes.ReparsePoint,
    };

    private Matcher _nameMatcher = new Matcher();
    private Matcher _pathMatcher = new Matcher();

    private bool IsMatchExclude(DirectoryInfo directoryInfo)
    {
        // FIXME: _pathMatcher.Match(directoryInfo.FullName).HasMatches is always false
        return directoryInfo.FullName.Split(Path.DirectorySeparatorChar).Any(p => _nameMatcher.Match(p).HasMatches)
        || _pathMatcher.Match(directoryInfo.FullName).HasMatches;
    }

    /// <summary>
    /// Initializes a new instance of the RepositorySearcher class with the specified root directories.
    /// </summary>
    /// <param name="rootDirectories">An array of root directories to search for repositories.</param>
    public RecursiveRepositorySearcher(string[] rootDirectories) : this(rootDirectories, [], []) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="RecursiveRepositorySearcher"/> class.
    /// </summary>
    /// <param name="rootDirectories">The root directories to search in.</param>
    /// <param name="excludePaths">The paths to exclude from the search.</param>
    /// <param name="excludeNames">The names to exclude from the search.</param>
    public RecursiveRepositorySearcher(string[] rootDirectories, string[] excludePaths, string[] excludeNames)
    {
        RootDirectories = rootDirectories;
        ExcludePaths = excludePaths;
        ExcludeNames = excludeNames;
        _nameMatcher.AddIncludePatterns(ExcludeNames);
        _pathMatcher.AddIncludePatterns(ExcludePaths);
    }

    /// <summary>
    /// Initializes a new instance of the RepositorySearcher class with the logical drives as the root directories.
    /// </summary>
    public RecursiveRepositorySearcher() : this(Environment.GetLogicalDrives()) { }

    /// <summary>
    /// Searches for repositories within the root directories.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to cancel the search operation.</param>
    /// <returns>A parallel query of repository paths.</returns>
    public ParallelQuery<DirectoryInfo> Search(CancellationToken cancellationToken)
    {
        return RootDirectories
        .AsParallel()
        .WithCancellation(cancellationToken)
        .SelectMany(d => Directory.EnumerateDirectories(d, RootSearchPattern, _rootEnumerationOptions))
        .AsParallel() // Somehow faster with this additional AsParallel()
        .WithCancellation(cancellationToken)
        .Where(d => !IsMatchExclude(new DirectoryInfo(d)))
        .SelectMany(d => Directory.EnumerateDirectories(d, SearchPattern, _enumerationOptions))
        .Select(d => Directory.GetParent(d))
        .Where(d => d != null)
        .Select(d => d ?? throw new InvalidOperationException("Directory.GetParent returns null"))
        .Where(d => !IsMatchExclude(d));
        // .Select(d => d!.FullName.Replace("\\", "/"));
    }
}
