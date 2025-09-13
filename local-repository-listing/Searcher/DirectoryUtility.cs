using Microsoft.Extensions.FileSystemGlobbing;
using ZLinq;

namespace LocalRepositoryListing.Searcher;

public static class DirectoryUtility
{

    /// <summary>
    /// Gets the normalized path of the specified directory.
    /// </summary>
    /// <param name="directoryInfo">The <see cref="DirectoryInfo"/> object representing the directory.</param>
    /// <returns>The normalized path of the directory.</returns>
    /// <remarks>
    /// The method replaces the directory separator characters with the alternate directory separator character and trims the ending directory separator.
    /// </remarks>
    public static string GetNormalizedPath(this DirectoryInfo directoryInfo)
    {
        return NormalizePath(directoryInfo.FullName);
    }

    /// <summary>
    /// Normalizes the specified path by replacing the directory separator characters with the alternate directory separator character and trimming the ending directory separator.
    /// </summary>
    /// <param name="path">The path to normalize.</param>
    /// <returns>The normalized path.</returns>
    public static string NormalizePath(string path)
    {
        return Path.TrimEndingDirectorySeparator(path.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
    }

    /// <summary>
    /// Determines if the specified directory matches the exclusion criteria.
    /// </summary>
    /// <param name="directoryInfo">The <see cref="DirectoryInfo"/> object representing the directory to check.</param>
    /// <param name="excludePaths">The repository paths to exclude from the search.</param>
    /// <param name="excludeNames">The repository names to exclude from the search.</param>
    /// <returns><c>true</c> if the directory matches the exclusion criteria; otherwise, <c>false</c>.</returns>
    /// <remarks>
    /// The method checks if the directory path contains any of the specified paths to exclude or if the directory name matches any of the specified names to exclude.
    /// </remarks>
    public static bool IsMatchExclude(DirectoryInfo directoryInfo, IReadOnlyCollection<string> excludePaths, IReadOnlyCollection<string> excludeNames)
    {
        if (directoryInfo == null || !directoryInfo.Exists)
        {
            return true;
        }


        if (excludePaths.Count == 0 && excludeNames.Count == 0)
        {
            return false;
        }

        var directoryPath = directoryInfo.GetNormalizedPath();

        if (excludePaths.Any(p => directoryPath.StartsWith(NormalizePath(p) + Path.AltDirectorySeparatorChar) || directoryPath == NormalizePath(p)))
        {
            return true;
        }

        var nameMatcher = new Matcher();
        nameMatcher.AddIncludePatterns(excludeNames);

        var isMatchName = directoryPath
                        .Split(Path.DirectorySeparatorChar)
                        .Where(p => !string.IsNullOrEmpty(p))
                        .Any(p => nameMatcher.Match(p).HasMatches);

        if (isMatchName)
        {
            return true;
        }

        return false;
    }
}
