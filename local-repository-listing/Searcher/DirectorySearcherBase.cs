using R3;

namespace LocalRepositoryListing.Searcher;

public abstract class DirectorySearcherBase : ISearcher, IDisposable

{
    protected readonly Subject<DirectoryInfo> _searchSubject = new();
    protected static readonly string _rootSearchPattern = "*";
    protected static readonly string _searchPattern = ".git";

    public Observable<DirectoryInfo> SearchResults { get; }

    /// <summary>
    /// Gets the root directories to search in.
    /// </summary>
    public IReadOnlyCollection<string> RootDirectories { get; }

    /// <summary>
    /// Gets the paths to exclude from the search.
    /// </summary>
    public IReadOnlyCollection<string> ExcludePaths { get; }

    /// <summary>
    /// Gets the directory names to exclude from the search.
    /// </summary>
    public IReadOnlyCollection<string> ExcludeNames { get; }

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
        SearchResults = _searchSubject;
    }


    /// <summary>
    /// Gets the enumeration options for the search.
    /// </summary>
    protected abstract EnumerationOptions EnumerationOptions { get; }

    public abstract Task Search(CancellationToken cancellationToken);

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _searchSubject?.Dispose();
        }
    }

}
