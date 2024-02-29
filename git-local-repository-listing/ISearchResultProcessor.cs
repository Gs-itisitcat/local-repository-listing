namespace ListLocalRepositories;

public interface ISearchResultProcessor
{
    /// <summary>
    /// Processes the search result.
    /// </summary>
    /// <param name="searchResult">The search result.</param>
    /// <param name="searchCancellationTokenSource">The cancellation token source for the search operation.</param>
    /// <returns>Exit code of the search result processor.</returns>
    public int ProcessSearchResult(ParallelQuery<DirectoryInfo> searchResult, CancellationTokenSource searchCancellationTokenSource);
}
