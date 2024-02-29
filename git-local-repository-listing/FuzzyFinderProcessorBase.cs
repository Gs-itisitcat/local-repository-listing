using System.Collections.ObjectModel;
using System.Diagnostics;

namespace ListLocalRepositories.FuzzyFinder;

/// <summary>
/// Represents the base class for a fuzzy finder process.
/// </summary>
public abstract class FuzzyFinderProcessorBase: ISearchResultProcessor
{
    /// <summary>
    /// Gets the name of the fuzzy finder.
    /// </summary>
    public abstract string FuzzyFinderName { get; }

    /// <summary>
    /// Gets the arguments for the processor.
    /// </summary>
    public ReadOnlyCollection<string> Arguments => _arguments.AsReadOnly();
    private string[] _arguments = [];

    private ProcessStartInfo _processStartInfo;

    /// <summary>
    /// Initializes a new instance of the <see cref="FuzzyFinderProcessorBase"/> class.
    /// </summary>
    public FuzzyFinderProcessorBase(string[] arguments)
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

    /// <summary>
    /// Processes the search result by starting the fuzzy finder process, writing the found repositories to its standard input,
    /// and waiting for the process to exit. If selection or cancellation occurs in fuzzy finder, the repository search is interrupted.
    /// </summary>
    /// <param name="searched">The parallel query of DirectoryInfo objects representing the searched repositories.</param>
    /// <param name="searchCancellationTokenSource">The CancellationTokenSource used to cancel the repository search.</param>
    /// <returns>The exit code of the fuzzy finder process.</returns>
    public int ProcessSearchResult(ParallelQuery<DirectoryInfo> searched, CancellationTokenSource searchCancellationTokenSource)
    {
        using var process = Process.Start(_processStartInfo);
        if (process == null)
        {
            Console.Error.WriteLine($"Failed to start {FuzzyFinderName}");
            return 1;
        }

        // Get the standard input of the redirected fuzzy finder process
        // For thread safety, use a synchronized wrapper around the StandardInput stream
        using var input = TextWriter.Synchronized(process.StandardInput);

        if (input == null)
        {
            Console.Error.WriteLine($"Failed to get StandardInput of {FuzzyFinderName}");
            return 1;
        }

        // Write the found repositories to the standard input while fuzzy finder is running
        _ = Task.Run(() =>
            searched
            .ForAll(d =>
                input.WriteLine(d.FullName.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar))
            ), searchCancellationTokenSource.Token);

        // WaitForExitAsync() causes OperationCanceledException and prevents fuzzy finder from starting for some reason
        process.WaitForExit();
        // If selection or cancellation occurs in fuzzy finder, interrupt the repository search even if it is not finished
        searchCancellationTokenSource.Cancel();

        return process.ExitCode;
    }
}
