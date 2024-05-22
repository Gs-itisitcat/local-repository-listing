using LocalRepositoryListing.Searcher;

namespace LocalRepositoryListing.ResultProcessor;

/// <summary>
/// Represents the processor for the FZF fuzzy finder.
/// </summary>
/// <seealso cref="FuzzyFinderListerBase" />
/// <seealso cref="IResultLister" />
public class FZFLister(ISearcher searcher, string? searchPattern, string[] args) : FuzzyFinderListerBase(searcher, arguments: [
            "--ansi",
            "--header",
            "\"Select a git repository\"",
            "--reverse",
            "--preview",
            "\"git -C {} branch  --color=always -a\"",
            "--preview-window",
            "\"right:30%\"",
            "--bind",
            "\"ctrl-]:change-preview-window(70%|30%)\"",
            "--bind",
            "\"?:preview:git -C {} log --color=always --graph --all --pretty=format:'%C(auto)%<(30,trunc)%s %C(cyan)%cr %C(auto)%d' \"",
            "--bind",
            "\"alt-?:preview:git -C {} branch  --color=always -a \"",
            "--query",
            $"{(string.IsNullOrWhiteSpace(searchPattern) ? "\"\"" : searchPattern)}",
            ..args
    ]), IResultLister
{
    private static readonly string _fuzzyFinderName = "fzf";
    public override string FuzzyFinderName => _fuzzyFinderName;
}
