using LocalRepositoryListing.Searcher;
using R3;
namespace LocalRepositoryListing.ResultLister;

/// <summary>
/// Initializes a new instance of the <see cref="ConsoleOutputLister"/> class with the specified search pattern.
/// </summary>
/// <param name="searcher">The <see cref="ISearcher"/> object representing the searcher.</param>
/// <param name="searchPattern">The search pattern to match against the full names of the directories.</param>
public class ConsoleOutputLister(ISearcher searcher, string searchPattern) : IResultLister
{
    /// <summary>
    /// The search pattern to match against the full names of the directories.
    /// </summary>
    private readonly string _searchPattern = searchPattern;
    private readonly ISearcher _searcher = searcher;

    public async ValueTask<int> ExecuteListingAsync(CancellationToken cancellationToken)
    {
        using var searchSubscription = _searcher.SearchResults.Subscribe(d =>
        {
            var fullName = d.GetNormalizedPath();

            if (string.IsNullOrEmpty(fullName) || (!string.IsNullOrEmpty(_searchPattern) && !fullName.Contains(_searchPattern)))
            {
                return;
            }

            Console.WriteLine(fullName);
        });

        cancellationToken.Register(searchSubscription.Dispose);

        try
        {
            var searchTask = _searcher.Search(cancellationToken);

            while (
                !cancellationToken.IsCancellationRequested
                && !searchTask.IsCompleted
                && !searchTask.IsFaulted
                && !searchTask.IsCanceled
                )
            {
                await Task.Delay(100, cancellationToken);
            }
        }
        catch (OperationCanceledException)
        {
            searchSubscription.Dispose();
            Console.Error.WriteLine("Search was cancelled.");
            return 1;
        }
        searchSubscription.Dispose();

        return 0;
    }
}
