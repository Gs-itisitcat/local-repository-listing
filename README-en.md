# local-repository-listing

[JA](README.md) | EN

This tool searches for git repositories on your local computer, selects them using a fuzzy finder, and navigates to the selected repository.

## Installation

Download from [Release](https://github.com/Gs-itisitcat/local-repository-listing/releases) and place it in any directory. After adding lepol to your PATH, source lepos.bash in .bashrc or similar.

```bash
source /path/to/lepos.bash
```

## Dependencies

- Fuzzy finder for selection (currently only fzf is supported)
- .NET 8 runtime (only when using the runtime dependent version)

## Command

```bash
lepos [options] [query]
```

- No arguments: Recursively search from the root of all drives.
- With arguments: Recursively search from the root of all drives and query repositories containing the directory name specified by the argument.

### Flags

- --root/-r: Specify the directory path to search. If not specified, recursively search from the root of all drives.
- --non-recursive/-n: Do not search recursively.
- --list-only/-l: Only output the search results.
- --exclude-name/-e: Specify the directory name to exclude from the search (multiple can be specified).
- --exclude-path/-ep: Specify the directory path to exclude from the search (multiple can be specified).
- --fuzzy-finder-args/-a: Specify arguments to pass to fzf (multiple can be specified).

## Fuzzy Finder Args

The default arguments passed to fzf are set.

```bash

fzf --ansi --header "\"Select a git repository\"" --reverse --preview "git -C {} branch  --color=always -a" --preview-window "right:30%" --bind "ctrl-]:change-preview-window(70%|30%)" --bind "?:preview:git -C {} log --color=always --graph --all --pretty=format:'%C(auto)%<(30,trunc)%s %C(cyan)%cr %C(auto)%d' " --bind "alt-?:preview:git -C {} branch  --color=always -a "

```

If you want to add arguments to fzf, use `--fuzzy-finder-args`.

```bash
lepos --fuzzy-finder-args "--no-reverse"
```

### Preview Window

The default preview window is set to displays a list of branches in the repository. \
You can also view the commit log by pressing `?` (`shift-/`). \
You can also view the branch list by pressing `alt-?` (`alt-shift-/`).
You can change the size of the preview window by pressing `ctrl-]`.
