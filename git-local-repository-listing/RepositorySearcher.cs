namespace ListLocalRepositories.Search;

/// <summary>
/// Represents a class that searches for local repositories within specified root directories.
/// </summary>
public class RepositorySearcher
{
    private static string RootSearchPattern = "*";
    private static string SearchPattern = ".git";
    public string[] RootDirectories { get; private set; }

    private EnumerationOptions _rootEnumerationOptions = new EnumerationOptions()
    {
        RecurseSubdirectories = false,
        IgnoreInaccessible = true,
        MatchType = MatchType.Simple,
        ReturnSpecialDirectories = false,
        AttributesToSkip = FileAttributes.System | FileAttributes.Compressed | FileAttributes.Offline | FileAttributes.Temporary | FileAttributes.ReparsePoint,
    };

    private EnumerationOptions _enumerationOptions = new EnumerationOptions()
    {
        RecurseSubdirectories = true,
        IgnoreInaccessible = true,
        MatchType = MatchType.Simple,
        ReturnSpecialDirectories = false,
        AttributesToSkip = FileAttributes.System | FileAttributes.Compressed | FileAttributes.Offline | FileAttributes.Temporary | FileAttributes.ReparsePoint,
    };

    /// <summary>
    /// Initializes a new instance of the RepositorySearcher class with the specified root directories.
    /// </summary>
    /// <param name="rootDirectories">An array of root directories to search for repositories.</param>
    public RepositorySearcher(string[] rootDirectories)
    {
        RootDirectories = rootDirectories;
    }

    /// <summary>
    /// Initializes a new instance of the RepositorySearcher class with the logical drives as the root directories.
    /// </summary>
    public RepositorySearcher() : this(Environment.GetLogicalDrives()) { }

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
        .SelectMany(d => Directory.EnumerateDirectories(d, RootSearchPattern, _rootEnumerationOptions))
        .AsParallel() // Somehow faster with this additional AsParallel()
        .WithCancellation(cancellationToken)
        .Where(d => !d.Contains("Windows.old")) // 一旦ハードコード
        .SelectMany(d => Directory.EnumerateDirectories(d, SearchPattern, _enumerationOptions))
        .Select(d => Directory.GetParent(d))
        .Where(d => d != null)
        .Select(d => d!.FullName.Replace("\\", "/"));
    }
}
