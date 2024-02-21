namespace ListLocalRepositories.FuzzyFinder;
/// <summary>
/// Represents an interface for a fuzzy finder process.
/// </summary>
public interface IFuzzyFinderProcess
{
    /// <summary>
    /// Represents an interface for a fuzzy finder process.
    /// </summary>
    public interface IFuzzyFinderProcess
    {
        /// <summary>
        /// Gets the name of the fuzzy finder.
        /// </summary>
        /// <value>The name of the fuzzy finder.</value>
        public string FuzzyFinderName { get; }

        /// <summary>
        /// Runs the fuzzy finder process on the specified searched directories.
        /// </summary>
        /// <param name="searchedDirectories">The directories to search in.</param>
        /// <param name="searchCancellationTokenSource">The cancellation token source for the search operation.</param>
        /// <returns>The exit code of the fuzzy finder process.</returns>
        public int Run(ParallelQuery<string> searchedDirectories, CancellationTokenSource searchCancellationTokenSource);
    }
}
