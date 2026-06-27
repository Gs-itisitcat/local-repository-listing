using System.Threading.Channels;
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
    /// Gets the paths to exclude from the search.
    /// </summary>
    public IReadOnlyCollection<string> ExcludePaths { get; }

    /// <summary>
    /// Searches for repositories based on the specified criteria.
    /// </summary>
    /// <param name="writer">The channel writer to write search results to.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the search operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task Search(ChannelWriter<DirectoryInfo> writer, CancellationToken cancellationToken);
}
