namespace LocalRepositoryListing.ResultProcessor;

public interface IResultLister
{
    /// <summary>
    /// Executes the listing operation.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="ValueTask{TResult}"/> representing the asynchronous operation, yielding the result of the listing operation.</returns>
    public ValueTask<int> ExecuteListing(CancellationToken cancellationToken);
}
