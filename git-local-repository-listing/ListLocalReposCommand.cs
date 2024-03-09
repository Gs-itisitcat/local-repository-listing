using ListLocalRepositories.Search;
using ListLocalRepositories.FuzzyFinder;

namespace ListLocalRepositories.Command;

/// <summary>
/// Represents a command for listing local repositories.
/// </summary>
public class ListLocalRepositoriesCommand : ConsoleAppBase
{
    public const string ArgumentDescription = "Search pattern for repositories";
    public const string RootDescription = """
    Root path of searching repositories.
                                            If not specified, the logical drives will be used as the root paths.
    """;
    public const string ListOnlyDescription = "List local repositories only";
    public const string NonRecursiveDescription = "Search for repositories non-recursively";
    public const string ExcludePathsDescription = "The paths to exclude from the search. Must be absolute paths.";
    public const string ExcludeNamesDescription = """
    The directory names to exclude from the search.
                                            Any Path that contains the directory of the name will be excluded.
                                            You can use glob patterns.
    """;
    public const string FuzzyFinderArgsDescription = "Arguments to pass to the fuzzy finder process.";

    /// <summary>
    /// Executes the list local repositories command.
    /// </summary>
    /// <param name="arg">Search pattern for repositories.</param>
    /// <param name="root">Root path of searching repositories.</param>
    /// <param name="listOnly">Flag to list local repositories only.</param>
    /// <param name="nonRecursive">Flag to search for repositories non-recursively.</param>
    /// <param name="excludePaths">The paths to exclude from the search. Must be absolute paths.</param>
    /// <param name="excludeNames">The directory names to exclude from the search. Any Path that contains the directory of the name will be excluded. You can use glob patterns.</param>
    /// <param name="fuzzyFinderArgs">Arguments to pass to the fuzzy finder process.</param>
    /// <returns>The result of the command execution.</returns>
    [RootCommand]
    public int Execute(
        [Option(0, ArgumentDescription)] string arg = "",
        [Option("r", RootDescription)] string root = "",
        [Option("l", ListOnlyDescription)] bool listOnly = false,
        [Option("n", NonRecursiveDescription)] bool nonRecursive = false,
        [Option("ep", ExcludePathsDescription)] string[]? excludePaths = null,
        [Option("e", ExcludeNamesDescription)] string[]? excludeNames = null,
        [Option("a", FuzzyFinderArgsDescription)] string[]? fuzzyFinderArgs = null
    )
    {
        var rootDirectories = string.IsNullOrEmpty(root) ? Environment.GetLogicalDrives() : [root];


        ISearcher searcher = nonRecursive
            ? new NonRecursiveRepositorySearcher(rootDirectories, excludePaths ?? [], excludeNames ?? [])
            : new RecursiveRepositorySearcher(rootDirectories, excludePaths ?? [], excludeNames ?? []);

        ISearchResultProcessor processor = listOnly
            ? new ConsoleOutputProcessor(arg)
            : new FZFProcessor(arg, fuzzyFinderArgs ?? []);


        // Pass the cancellation token source of search cancellation token to the fuzzy finder process
        // to cancel the search when the fuzzy finder process is terminated.
        var cts = new CancellationTokenSource();
        Context.CancellationToken.Register(cts.Cancel);
        return processor.ProcessSearchResult(searcher.Search(cts.Token), cts);
    }
}
