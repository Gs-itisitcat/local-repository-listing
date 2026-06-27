using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Channels;
using LocalRepositoryListing.Searcher;

namespace LocalRepositoryListing.ResultLister;

/// <summary>
/// Represents the base class for a fuzzy finder process.
/// </summary>
public abstract class FuzzyFinderListerBase : IResultLister
{
    /// <summary>
    /// Gets the name of the fuzzy finder.
    /// </summary>
    public abstract string FuzzyFinderName { get; }

    /// <summary>
    /// Gets the arguments for the processor.
    /// </summary>
    public ReadOnlyCollection<string> Arguments => _arguments.AsReadOnly();
    private readonly string[] _arguments = [];

    private readonly ProcessStartInfo _processStartInfo;

    /// <summary>
    /// Initializes a new instance of the <see cref="FuzzyFinderListerBase"/> class.
    /// </summary>
    /// <param name="searcher">The <see cref="ISearcher"/> object representing the searcher.</param>
    /// <param name="arguments">The arguments for the processor.</param>
    public FuzzyFinderListerBase(string[] arguments)
    {
        _arguments = arguments;
        _processStartInfo = new ProcessStartInfo(FuzzyFinderName)
        {
            // UseShellExecute is set to false to start the child process without using a shell
            UseShellExecute = false,
            RedirectStandardInput = true,
            // For Non-ASCII characters
            StandardInputEncoding = System.Text.Encoding.UTF8,
            Arguments = string.Join(" ", arguments),
        };
    }

    public async ValueTask<int> ExecuteListingAsync(ChannelReader<DirectoryInfo> reader, CancellationToken cancellationToken)
    {
        using var process = Process.Start(_processStartInfo);
        if (process == null)
        {
            Console.Error.WriteLine($"Failed to start {FuzzyFinderName}");
            return 1;
        }

        using var input = TextWriter.Synchronized(process.StandardInput);
        if (input == null)
        {
            Console.Error.WriteLine($"Failed to get StandardInput of {FuzzyFinderName}");
            return 1;
        }

        var running = process.WaitForExitAsync(cancellationToken);

        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        process.Exited += (sender, e) => linkedCts.Cancel();

        await foreach (var directory in reader.ReadAllAsync(linkedCts.Token))
        {
            var fullName = directory.GetNormalizedPath();
            if (string.IsNullOrEmpty(fullName))
            {
                continue;
            }

            if (process.HasExited)
            {
                break;
            }

            input.WriteLine(fullName);
        }

        await running;
        return process.ExitCode;
    }
}
