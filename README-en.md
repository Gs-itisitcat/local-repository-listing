# git-local-repository-listing

[JA](README.md) | EN

This tool searches for git repositories on your local computer, selects them using a fuzzy finder, and navigates to the selected repository.

## Installation

Download from [Release](https://github.com/Gs-itisitcat/git-local-repository-listing/releases) and place it in any directory. Source `lepos.bash` in your `.bashrc` or similar.

```bash
source /path/to/lepos.bash
```

## Command

```bash
lepos [options] [query]
```

- No arguments: Recursively search from the root of all drives.
- With arguments: Recursively search from the root of all drives and query repositories containing the directory name specified by the argument.

### Flags

- --root/-r: Specify the directory path to search.
- --non-recursive/-n: Do not search recursively.
- --list-only/-l: Only output the search results.
- --exclude-name/-e: Specify the directory name to exclude from the search (multiple can be specified).
- --exclude-path/-ep: Specify the directory path to exclude from the search (multiple can be specified).
- --fuzzy-finder-args/-a: Specify arguments to pass to fzf (multiple can be specified).