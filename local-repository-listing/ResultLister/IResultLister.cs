using System.Threading.Channels;

namespace LocalRepositoryListing.ResultLister;

public interface IResultLister
{
    /// <summary>
    /// Executes the listing operation.
    /// </summary>
    /// <param name="reader">The channel reader to read search results from.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="ValueTask{TResult}"/> representing the asynchronous operation, yielding the result of the listing operation.</returns>
    public ValueTask<int> ExecuteListingAsync(ChannelReader<DirectoryInfo> reader, CancellationToken cancellationToken);
}
