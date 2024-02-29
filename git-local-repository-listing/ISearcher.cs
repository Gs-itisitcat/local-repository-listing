namespace ListLocalRepositories;

public interface ISearcher
{
    public ParallelQuery<DirectoryInfo> Search(CancellationToken cancellationToken);
}
