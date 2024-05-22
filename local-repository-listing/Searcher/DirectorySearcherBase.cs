
using Microsoft.Extensions.FileSystemGlobbing;

namespace LocalRepositoryListing.Searcher;

public abstract class DirectorySearcherBase : ISearcher
{
    protected static readonly string _rootSearchPattern = "*";
    protected static readonly string _searchPattern = ".git";
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

    public DirectorySearcherBase(IList<string> rootDirectories, IList<string> excludePaths, IList<string> excludeNames)
    {
        RootDirectories = rootDirectories.AsReadOnly();
        ExcludePaths = excludePaths.AsReadOnly();
        ExcludeNames = excludeNames.AsReadOnly();
        _nameMatcher.AddIncludePatterns(excludeNames);
    }

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

    public abstract ParallelQuery<DirectoryInfo> Search(CancellationToken cancellationToken);
}
