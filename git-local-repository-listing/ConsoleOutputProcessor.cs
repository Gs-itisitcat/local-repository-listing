namespace ListLocalRepositories;

public class ConsoleOutputProcessor : ISearchResultProcessor
{
    public int ProcessSearchResult(ParallelQuery<DirectoryInfo> searchResult, CancellationTokenSource searchCancellationTokenSource)
    {
        try
        {
            var searchTask = Task.Run(() =>
                searchResult
                .ForAll(d =>
                    Console.WriteLine(d.FullName.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar))
                    ), searchCancellationTokenSource.Token
            );

            while (
                !searchCancellationTokenSource.Token.IsCancellationRequested
                && !searchTask.IsCompleted
                && !searchTask.IsFaulted
                && !searchTask.IsCanceled
            )
            {
                Task.Delay(100).Wait();
            }
        }
        catch (OperationCanceledException)
        {
            Console.Error.WriteLine("Search was cancelled");
            return 1;
        }

        if (searchCancellationTokenSource.Token.IsCancellationRequested)
        {
            Console.Error.WriteLine("Search was cancelled");
            return 1;
        }
        return 0;
    }
}
