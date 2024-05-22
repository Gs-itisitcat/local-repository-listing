namespace LocalRepositoryListing.Searcher;
/// <summary>
/// Represents a searcher that can search for repositories based on specified criteria.
/// </summary>
public interface ISearcher
{
    /// <summary>
    /// Gets the directory names to exclude from the search.
    /// </summary>
    public IReadOnlyCollection<string> ExcludeNames { get; }

    /// <summary>
    /// Searches for repositories based on the specified criteria.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to cancel the search operation.</param>
    /// <returns>A parallel query of <see cref="DirectoryInfo"/> objects representing the search results.</returns>
    public ParallelQuery<DirectoryInfo> Search(CancellationToken cancellationToken);
}
