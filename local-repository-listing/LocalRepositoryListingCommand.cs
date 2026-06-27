using LocalRepositoryListing.Searcher;
using LocalRepositoryListing.ResultLister;
using ConsoleAppFramework;
using System.Threading.Channels;

namespace LocalRepositoryListing;

/// <summary>
/// Represents a command for listing local repositories.
/// </summary>
internal class LocalRepositoryListingCommand
{
    /// <summary>
    /// Lists local git repositories.
    ///
    /// Searches for local git repositories based on the specified criteria and lists the results.
    /// By default, the search is performed recursively, and for all logical drives except for network drives.
    /// </summary>
    /// <param name="root">-r,Root path of searching repositories.
    ///                                     Relative paths are supported.
    ///                                     If not specified, the logical drives will be used as the root paths,
    ///                                     except for network drives.</param>
    /// <param name="listOnly">-l,Flag to list local repositories only.</param>
    /// <param name="nonRecursive">-n,Flag to search for repositories non-recursively.</param>
    /// <param name="excludePaths">-ep,The paths to exclude from the search. Must be absolute paths.</param>
    /// <param name="excludeNames">-e,The directory names to exclude from the search.
    ///                                    Any Path that contains the directory of the name will be excluded.
    ///                                    You can use glob patterns.</param>
    /// <param name="fuzzyFinderArgs">-a,Arguments to pass to the fuzzy finder process.</param>
    /// <param name="cancellationToken">A cancellation token to handle SIGINT/SIGTERM/SIGKILL - Ctrl+C.</param>
    /// <param name="args">Search patterns.</param>
    /// <returns>The result of the command execution.</returns>
    [Command("")]
    public async Task<int> Lepol(
        [HideDefaultValue] string? root = null,
        bool listOnly = false,
        bool nonRecursive = false,
        [HideDefaultValue] string[]? excludePaths = null,
        [HideDefaultValue] string[]? excludeNames = null,
        [HideDefaultValue] string[]? fuzzyFinderArgs = null,
        CancellationToken cancellationToken = default,
        [Argument] params string[] args
    )
    {
        var rootDirectories = string.IsNullOrEmpty(root)
            ? DriveInfo.GetDrives()
                .Where(d => d.IsReady && d.DriveType != DriveType.Network)
                .Select(d => d.Name)
                .ToArray()
            : [root];

        if (excludePaths != null && excludePaths.Any(p => !Path.IsPathRooted(p)))
        {
            Console.Error.WriteLine("Ignore the relative paths in excludePaths. Please specify absolute paths.");
            excludePaths = [.. excludePaths.Where(p => Path.IsPathRooted(p))];
        }

        var searcher = new EnumerateDirectorySearcher(rootDirectories, excludePaths ?? [], excludeNames ?? [])
        {
            RecurseSubdirectories = !nonRecursive,
        };

        IResultLister listable = listOnly
            ? new ConsoleOutputLister(args)
            : new FZFLister(args, fuzzyFinderArgs ?? []);

        var channel = Channel.CreateUnbounded<DirectoryInfo>(new UnboundedChannelOptions
        {
            SingleReader = true,
            SingleWriter = false,
        });

        var listing = listable.ExecuteListingAsync(channel.Reader, cancellationToken);
        using var listingFinishedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        var searching = Search(searcher, channel.Writer, listingFinishedCts.Token);
        var exitCode = await listing;
        listingFinishedCts.Cancel();

        return exitCode;
    }

    private static async Task Search(ISearcher searcher, ChannelWriter<DirectoryInfo> writer, CancellationToken cancellationToken = default)
    {
        await searcher.Search(writer, cancellationToken);
        writer.Complete();
    }
}
