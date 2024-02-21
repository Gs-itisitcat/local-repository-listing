using System.Diagnostics;

namespace ListLocalRepositories.FuzzyFinder;

public class FZFProcess : IFuzzyFinderProcess
{
    public string FuzzyFinderName { get; private set; } = "fzf";
    public string[] Arguments { get; private set; } = [];

    private ProcessStartInfo _processStartInfo;

    public FZFProcess(string? searchPattern)
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

        _processStartInfo = new ProcessStartInfo(FuzzyFinderName)
        {
            // UseShellExecute is set to false to start the child process without using a shell
            UseShellExecute = false,

            RedirectStandardInput = true,
            // For Non-ASCII characters
            StandardInputEncoding = System.Text.Encoding.UTF8,

            Arguments = string.Join(" ", Arguments),
        };
    }


    public int Run(ParallelQuery<string> searched, CancellationTokenSource searchCancellationTokenSource)
    {
        using var process = Process.Start(_processStartInfo);
        if (process == null)
        {
            Console.Error.WriteLine($"Failed to start {FuzzyFinderName}");
            return 1;
        }

        // Get the standard input of the redirected fzf process
        // For thread safety, use a synchronized wrapper around the StandardInput stream
        using var fzfInput = TextWriter.Synchronized(process.StandardInput);

        if (fzfInput == null)
        {
            Console.Error.WriteLine($"Failed to get StandardInput of {FuzzyFinderName}");
            return 1;
        }

        // fzfの起動中に逐次見つけたリポジトリを標準入力に書き込む
        _ = Task.Run(() => searched.ForAll(fzfInput.WriteLine), searchCancellationTokenSource.Token);

        // WaitForExitAsync()を使うと何故かOperationCanceledExceptionが発生してfzfが起動しない
        process.WaitForExit();
        // fzf側で選択あるいはキャンセルされたらリポジトリ検索が終わっていなくても中断
        searchCancellationTokenSource.Cancel();

        return process.ExitCode;

    }

}
