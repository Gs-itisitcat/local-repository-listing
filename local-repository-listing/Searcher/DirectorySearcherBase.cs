
using Microsoft.Extensions.FileSystemGlobbing;
using R3;

namespace LocalRepositoryListing.Searcher;

public abstract class DirectorySearcherBase : ISearcher
{
    protected readonly Subject<DirectoryInfo> _searchResults = new();
    protected static readonly string _rootSearchPattern = "*";
    protected static readonly string _searchPattern = ".git";

    public Observable<DirectoryInfo> SearchResults { get; }

    /// <summary>
    /// Gets the root directories to search in.
    /// </summary>
    public IReadOnlyCollection<string> RootDirectories { get; init; }

    /// <summary>
    /// Gets the paths to exclude from the search.
    /// </summary>
    public IReadOnlyCollection<string> ExcludePaths { get; init; }

    /// <summary>
    /// Gets the directory names to exclude from the search.
    /// </summary>
    public IReadOnlyCollection<string> ExcludeNames { get; init; }

    private readonly Matcher _nameMatcher = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="DirectorySearcherBase"/> class with the specified root directories, paths to exclude, and names to exclude.
    /// </summary>
    /// <param name="rootDirectories">The root directories to search in.</param>
    /// <param name="excludePaths">The repository paths to exclude from the search.</param>
    /// <param name="excludeNames">The repository names to exclude from the search.</param>
    public DirectorySearcherBase(IList<string> rootDirectories, IList<string> excludePaths, IList<string> excludeNames)
    {
        RootDirectories = rootDirectories.AsReadOnly();
        ExcludePaths = excludePaths.AsReadOnly();
        ExcludeNames = excludeNames.AsReadOnly();
        _nameMatcher.AddIncludePatterns(excludeNames);
        SearchResults = _searchResults;
    }


    /// <summary>
    /// Gets the enumeration options for the search.
    /// </summary>
    protected abstract EnumerationOptions EnumerationOptions { get; }

    /// <summary>
    /// Determines if the specified directory matches the exclusion criteria.
    /// </summary>
    /// <param name="directoryInfo">The <see cref="DirectoryInfo"/> object representing the directory to check.</param>
    /// <returns><c>true</c> if the directory matches the exclusion criteria; otherwise, <c>false</c>.</returns>
    protected bool IsMatchExclude(DirectoryInfo directoryInfo)
    {
        return directoryInfo.FullName
                .Split(Path.DirectorySeparatorChar)
                .Where(p => !string.IsNullOrEmpty(p))
                .Any(p => _nameMatcher.Match(p).HasMatches)
        || ExcludePaths
            .Any(p => directoryInfo.FullName
                            .Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                            .Contains(p)
                );
    }

    public abstract Task Search(CancellationToken cancellationToken);
}
