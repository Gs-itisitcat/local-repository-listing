namespace LocalRepositoryListing.ResultProcessor;

public class FZFProcessor(string? searchPattern, string[] args) : FuzzyFinderProcessorBase(arguments: [
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
    ])
{
    private static readonly string _fuzzyFinderName = "fzf";
    public override string FuzzyFinderName => _fuzzyFinderName;
}
