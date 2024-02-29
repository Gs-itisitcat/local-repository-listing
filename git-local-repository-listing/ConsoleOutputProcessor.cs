namespace ListLocalRepositories;

public class ConsoleOutputProcessor : ISearchResultProcessor
{
    /// <summary>
    /// Processes the search result by printing the full names of the directories to the console.
    /// </summary>
    /// <param name="searchResult">The search result containing the directories to be processed.</param>
    /// <param name="searchCancellationTokenSource">The cancellation token source used to cancel the search.</param>
    /// <returns>0 if the search result was processed successfully, 1 if the search was cancelled.</returns>
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
