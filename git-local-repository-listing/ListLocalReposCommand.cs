using ListLocalRepositories.Search;
using ListLocalRepositories.FuzzyFinder;

namespace ListLocalRepositories.Command;

/// <summary>
/// Represents a command for listing local repositories.
/// </summary>
public class ListLocalRepositoriesCommand : ConsoleAppBase
{
    private const string ArgumentDescription = "Search pattern for repositories";
    private const string RootDescription = "Root path of searching repositories";
    private const string ListOnlyDescription = "List local repositories only";
    private const string NonRecursiveDescription = "Search for repositories non-recursively";
    private const string ExcludePathsDescription = "The paths to exclude from the search";
    private const string ExcludeNamesDescription = "The directory names to exclude from the search";

    /// <summary>
    /// Executes the command to list local repositories.
    /// </summary>
    /// <param name="arg">The search pattern for repositories.</param>
    /// <param name="root">The root path of searching repositories.</param>
    /// <returns>The exit code of the command.</returns>
    [RootCommand]
    public int Execute(
        [Option(0, ArgumentDescription)] string arg = "",
        [Option("r", RootDescription)] string root = "",
        [Option("l", ListOnlyDescription)] bool listOnly = false,
        [Option("n", NonRecursiveDescription)] bool nonRecursive = false,
        [Option("e", ExcludePathsDescription)] string[]? excludePaths = null,
        [Option("E", ExcludeNamesDescription)]string[]? excludeNames = null
    )
    {
        var rootDirectories = string.IsNullOrEmpty(root) ? Environment.GetLogicalDrives() : [root];


        ISearcher searcher = nonRecursive
            ? new NonRecursiveRepositorySearcher(rootDirectories, excludePaths ?? [], excludeNames ?? [])
            : new RecursiveRepositorySearcher(rootDirectories, excludePaths ?? [], excludeNames ?? []);

        ISearchResultProcessor processor = listOnly
            ? new ConsoleOutputProcessor()
            : new FZFProcessor(arg);


        // Pass the cancellation token source of search cancellation token to the fuzzy finder process
        // to cancel the search when the fuzzy finder process is terminated.
        var cts = new CancellationTokenSource();
        return processor.ProcessSearchResult(searcher.Search(cts.Token), cts);
    }
}
