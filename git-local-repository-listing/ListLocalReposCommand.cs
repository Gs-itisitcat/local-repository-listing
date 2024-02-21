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

    /// <summary>
    /// Executes the command to list local repositories.
    /// </summary>
    /// <param name="arg">The search pattern for repositories.</param>
    /// <param name="root">The root path of searching repositories.</param>
    /// <returns>The exit code of the command.</returns>
    [RootCommand]
    public int Execute(
        [Option(0, ArgumentDescription)] string arg = "",
        [Option("r", RootDescription)] string root = ""
    )
    {
        var rootDirectories = string.IsNullOrEmpty(root) ? Environment.GetLogicalDrives() : [root];

        var searcher = new RepositorySearcher(rootDirectories);
        var fzf = new FZFProcess(arg);

        // Pass the cancellation token source of search cancellation token to the fuzzy finder process
        // to cancel the search when the fuzzy finder process is terminated.
        var cts = new CancellationTokenSource();
        return fzf.Run(searcher.Search(cts.Token), cts);
    }
}
