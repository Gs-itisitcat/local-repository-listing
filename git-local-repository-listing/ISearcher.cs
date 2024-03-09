using System.Collections.ObjectModel;
namespace LocalRepositoryListing.Searcher;

/// <summary>
/// Represents a searcher that can search for repositories based on specified criteria.
/// </summary>
public interface ISearcher
{
    /// <summary>
    /// Gets the root directories to search in.
    /// </summary>
    public string[] RootDirectories { get; }

    /// <summary>
    /// Gets the paths to exclude from the search.
    /// </summary>
    public ReadOnlyCollection<string> ExcludePaths { get; }

    /// <summary>
    /// Gets the directory names to exclude from the search.
    /// </summary>
    public ReadOnlyCollection<string> ExcludeNames { get; }

    /// <summary>
    /// Searches for repositories based on the specified criteria.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to cancel the search operation.</param>
    /// <returns>A parallel query of <see cref="DirectoryInfo"/> objects representing the search results.</returns>
    public ParallelQuery<DirectoryInfo> Search(CancellationToken cancellationToken);
}
