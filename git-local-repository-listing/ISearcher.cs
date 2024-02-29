namespace ListLocalRepositories;

public interface ISearcher
{
    public string[] RootDirectories { get; }
    public string[] ExcludePaths { get; }
    public string[] ExcludeNames { get; }
    public ParallelQuery<DirectoryInfo> Search(CancellationToken cancellationToken);
}
