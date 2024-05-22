using R3;
namespace LocalRepositoryListing.Searcher;
/// <summary>
/// Represents a searcher that can search for repositories based on specified criteria.
/// </summary>
public interface ISearcher
{

    /// <summary>
    /// Gets the observable collection of search results.
    /// </summary>
    public Observable<DirectoryInfo> SearchResults { get; }

    /// <summary>
    /// Gets the directory names to exclude from the search.
    /// </summary>
    public IReadOnlyCollection<string> ExcludeNames { get; }

    /// <summary>
    /// Searches for repositories based on the specified criteria.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to cancel the search operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task Search(CancellationToken cancellationToken);
}
