namespace ListLocalRepositories;

public interface ISearchResultProcessor
{
    public int ProcessSearchResult(ParallelQuery<DirectoryInfo> searchResult, CancellationTokenSource searchCancellationTokenSource);
}
