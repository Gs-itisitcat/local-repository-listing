using System.Collections.ObjectModel;
using System.Diagnostics;
using LocalRepositoryListing.Searcher;
using R3;

namespace LocalRepositoryListing.ResultProcessor;

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
    private readonly ISearcher _searcher;

    /// <summary>
    /// Initializes a new instance of the <see cref="FuzzyFinderListerBase"/> class.
    /// </summary>
    /// <param name="searcher">The <see cref="ISearcher"/> object representing the searcher.</param>
    /// <param name="arguments">The arguments for the processor.</param>
    public FuzzyFinderListerBase(ISearcher searcher, string[] arguments)
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

        _searcher = searcher;
    }

    public ValueTask<int> ExecuteListing(CancellationToken cancellationToken)
    {
        using var process = Process.Start(_processStartInfo);
        if (process == null)
        {
            Console.Error.WriteLine($"Failed to start {FuzzyFinderName}");
            return ValueTask.FromResult(1);
        }

        using var input = TextWriter.Synchronized(process.StandardInput);
        if (input == null)
        {
            Console.Error.WriteLine($"Failed to get StandardInput of {FuzzyFinderName}");
            return ValueTask.FromResult(1);
        }

        using var searchSubscription = _searcher.SearchResults.Subscribe(d => input.WriteLine(d.FullName.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)));

        _ = _searcher.Search(cancellationToken);

        process.WaitForExit();

        return ValueTask.FromResult(process.ExitCode);
    }
}
