using LocalRepositoryListing.Searcher;
using LocalRepositoryListing.ResultLister;
using ConsoleAppFramework;

namespace LocalRepositoryListing;

/// <summary>
/// Represents a command for listing local repositories.
/// </summary>
internal class LocalRepositoryListingCommand
{
    /// <summary>
    /// Executes the list local repositories command.
    /// Arguments after `--` are treated as the search pattern.
    /// </summary>
    /// <param name="root">-r,Root path of searching repositories.
    ///                                    If not specified, the logical drives will be used as the root paths.</param>
    /// <param name="listOnly">-l,Flag to list local repositories only.</param>
    /// <param name="nonRecursive">-n,Flag to search for repositories non-recursively.</param>
    /// <param name="excludePaths">-ep,The paths to exclude from the search. Must be absolute paths.</param>
    /// <param name="excludeNames">-e,The directory names to exclude from the search.
    ///                                    Any Path that contains the directory of the name will be excluded.
    ///                                    You can use glob patterns.</param>
    /// <param name="fuzzyFinderArgs">-a,Arguments to pass to the fuzzy finder process.</param>
    /// <returns>The result of the command execution.</returns>
    [Command("")]
    public async Task<int> Lepol(
        CancellationToken cancellationToken,
        string root = "",
        bool listOnly = false,
        bool nonRecursive = false,
        string[]? excludePaths = null,
        string[]? excludeNames = null,
        string[]? fuzzyFinderArgs = null,
        ConsoleAppContext context = null!
    )
    {
        var rootDirectories = string.IsNullOrEmpty(root) ? Environment.GetLogicalDrives() : [root];

        ISearcher searcher = new EnumerateDirectorySearcher(rootDirectories, excludePaths ?? [], excludeNames ?? [])
        {
            RecurseSubdirectories = !nonRecursive,
        };

        var arg = context.EscapedArguments.ToArray() ?? [];

        IResultLister listable = listOnly
            ? new ConsoleOutputLister(searcher, string.Join(" ", arg))
            : new FZFLister(searcher, $"\"{string.Join(" ", arg)}\"", fuzzyFinderArgs ?? []);

        var exitCode = await listable.ExecuteListingAsync(cancellationToken);

        return exitCode;
    }
}
