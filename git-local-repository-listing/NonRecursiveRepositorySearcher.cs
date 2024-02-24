namespace ListLocalRepositories.Search;

/// <summary>
/// Represents a class that searches for local repositories within specified root directories.
/// </summary>
public class NonRecursiveRepositorySearcher : ISearcher
{
    public string[] RootDirectories { get; private set; }

    private EnumerationOptions _enumerationOptions = new EnumerationOptions()
    {
        RecurseSubdirectories = false,
        IgnoreInaccessible = true,
        MatchType = MatchType.Simple,
        ReturnSpecialDirectories = false,
        AttributesToSkip = FileAttributes.System | FileAttributes.Compressed | FileAttributes.Offline | FileAttributes.Temporary | FileAttributes.ReparsePoint,
    };

    /// <summary>
    /// Initializes a new instance of the RepositorySearcher class with the specified root directories.
    /// </summary>
    /// <param name="rootDirectories">An array of root directories to search for repositories.</param>
    public NonRecursiveRepositorySearcher(string[] rootDirectories)
    {
        RootDirectories = rootDirectories;
    }

    /// <summary>
    /// Searches for repositories within the root directories.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to cancel the search operation.</param>
    /// <returns>A parallel query of repository paths.</returns>
    public ParallelQuery<string> Search(CancellationToken cancellationToken)
    {
        return RootDirectories
        .AsParallel()
        .WithCancellation(cancellationToken)
        .SelectMany(d => Directory.EnumerateDirectories(d, "*", _enumerationOptions))
        .Select(d => new DirectoryInfo(d))
        .Where(d => d.GetDirectories(".git", SearchOption.TopDirectoryOnly).Any())
        .Select(d => d!.FullName.Replace("\\", "/"));
    }
}
