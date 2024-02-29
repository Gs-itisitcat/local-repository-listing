﻿using System.Collections.ObjectModel;
using Microsoft.Extensions.FileSystemGlobbing;
namespace ListLocalRepositories.Search;

/// <summary>
/// Represents a class that searches for local repositories within specified root directories.
/// </summary>
public class NonRecursiveRepositorySearcher : ISearcher
{
    public string[] RootDirectories { get; init; }
    public ReadOnlyCollection<string> ExcludePaths { get; init; }
    public ReadOnlyCollection<string> ExcludeNames { get; init; }

    private EnumerationOptions _enumerationOptions = new EnumerationOptions()
    {
        RecurseSubdirectories = false,
        IgnoreInaccessible = true,
        MatchType = MatchType.Simple,
        ReturnSpecialDirectories = false,
        AttributesToSkip = FileAttributes.System | FileAttributes.Compressed | FileAttributes.Offline | FileAttributes.Temporary | FileAttributes.ReparsePoint,
    };

    /// <summary>
    /// Initializes a new instance of the RepositorySearcher class with the specified root directories.
    /// </summary>
    /// <param name="rootDirectories">An array of root directories to search for repositories.</param>
    public NonRecursiveRepositorySearcher(string[] rootDirectories) : this(rootDirectories, [], []) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="NonRecursiveRepositorySearcher"/> class.
    /// </summary>
    /// <param name="rootDirectories">The root directories to search in.</param>
    /// <param name="excludePaths">The paths to exclude from the search.</param>
    /// <param name="excludeNames">The names to exclude from the search.</param>
    public NonRecursiveRepositorySearcher(string[] rootDirectories, string[] excludePaths, string[] excludeNames)
    {
        RootDirectories = rootDirectories;
        ExcludePaths = excludePaths.AsReadOnly();
        ExcludeNames = excludeNames.AsReadOnly();
        _nameMatcher.AddIncludePatterns(excludeNames);
    }

    private Matcher _nameMatcher = new Matcher();

    /// <summary>
    /// Determines if the specified directory matches the exclusion criteria.
    /// </summary>
    /// <param name="directoryInfo">The <see cref="DirectoryInfo"/> object representing the directory to check.</param>
    /// <returns><c>true</c> if the directory matches the exclusion criteria; otherwise, <c>false</c>.</returns>
    private bool IsMatchExclude(DirectoryInfo directoryInfo)
    {
        // FIXME: _pathMatcher.Match(directoryInfo.FullName).HasMatches is always false
        return directoryInfo.FullName.Split(Path.DirectorySeparatorChar).Any(p => _nameMatcher.Match(p).HasMatches)
        || ExcludePaths.Any(p => directoryInfo.FullName.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).Contains(p));
    }

    public ParallelQuery<DirectoryInfo> Search(CancellationToken cancellationToken)
    {
        return RootDirectories
        .AsParallel()
        .WithCancellation(cancellationToken)
        .SelectMany(d => Directory.EnumerateDirectories(d, "*", _enumerationOptions))
        .Select(d => new DirectoryInfo(d))
        .Where(d => d.GetDirectories(".git", SearchOption.TopDirectoryOnly).Any())
        .Where(d => !IsMatchExclude(d));
    }
}
