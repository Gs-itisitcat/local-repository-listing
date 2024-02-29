namespace ListLocalRepositories;

public class ConsoleOutputProcessor: ISearchResultProcessor
{
    public int ProcessSearchResult(ParallelQuery<DirectoryInfo> searchResult, CancellationTokenSource searchCancellationTokenSource)
    {
        searchResult.ForAll(d => Console.WriteLine(d.FullName));
        return 0;
    }
}
