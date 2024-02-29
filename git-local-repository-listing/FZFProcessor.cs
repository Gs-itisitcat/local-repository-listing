using System.Diagnostics;

namespace ListLocalRepositories.FuzzyFinder;

public class FZFProcessor : FuzzyFinderProcessorBase
{
    private static readonly string _fuzzyFinderName = "fzf";
    public override string FuzzyFinderName => _fuzzyFinderName;

    public FZFProcessor(string? searchPattern) : base()
    {
        Arguments = [
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
            "--query",
            $"{(string.IsNullOrWhiteSpace(searchPattern) ? "\"\"" : searchPattern)}"
            ];
    }
}
