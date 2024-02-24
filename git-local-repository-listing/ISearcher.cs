namespace ListLocalRepositories;

public interface ISearcher
{
    public ParallelQuery<string> Search(CancellationToken cancellationToken);
}
