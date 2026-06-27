using System.Threading.Channels;
using LocalRepositoryListing.Searcher;
namespace LocalRepositoryListing.ResultLister;

/// <summary>
/// Initializes a new instance of the <see cref="ConsoleOutputLister"/> class with the specified search pattern.
/// </summary>
/// <param name="searcher">The <see cref="ISearcher"/> object representing the searcher.</param>
/// <param name="searchPattern">The search pattern to match against the full names of the directories.</param>
public class ConsoleOutputLister(string[] searchPattern) : IResultLister
{
    /// <summary>
    /// The search pattern to match against the full names of the directories.
    /// </summary>
    private readonly string[] _searchPattern = searchPattern;

    public async ValueTask<int> ExecuteListingAsync(ChannelReader<DirectoryInfo> reader, CancellationToken cancellationToken)
    {
        try
        {
            await foreach (var directory in reader.ReadAllAsync(cancellationToken))
            {
                var fullName = directory.GetNormalizedPath();
                if (string.IsNullOrEmpty(fullName) || !_searchPattern.All(p => fullName.Contains(p, StringComparison.OrdinalIgnoreCase)))
                {
                    continue;
                }

                Console.WriteLine(fullName);
            }
        }
        catch (OperationCanceledException)
        {
            Console.Error.WriteLine("Search was cancelled.");
            return 130;
        }

        return 0;
    }
}
