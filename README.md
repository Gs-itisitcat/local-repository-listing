# local-repository-listing

JA | [EN](README-en.md)

List git local repository on your computer.

ローカルにあるgitリポジトリを検索し、fazzy finderで選択して移動するツール

## インストール

[Release](https://github.com/Gs-itisitcat/local-repository-listing/releases)からダウンロードして、任意のディレクトリに配置してください。
lepolにPATHを通したのち、lepos.bashを.bashrcなどにsourceしてください。

```bash
source /path/to/lepos.bash
```

## 依存関係

- 選択に使用するfuzzy finder (現在はfzfのみ対応)
- .NET 8 runtime (runtime dependent版使用時のみ)

## コマンド

```bash
lepos [options] [query]
```

- 引数無し: 全ドライブのルートから再帰的に検索
- 引数有り: 全ドライブのルートから再帰的に検索し、引数で指定したディレクトリ名を含むリポジトリをクエリ

### flags

- --root/-r: 検索するディレクトリパス。指定しない場合は全ドライブのルートから再帰的に検索
- --non-recursive/-n: 再帰的に検索しない
- --list-only/-l: 検索結果の出力のみ
- --exclude-name/-e: 検索対象から除外するディレクトリ名 (複数指定可)
- --exclude-path/-ep: 検索対象から除外するディレクトリパス (複数指定可)
- --fuzzy-finder-args/-a: fzfに渡す引数 (複数指定可)

## fuzzy finder args

デフォルトでfzfに渡す引数を設定しています。

```bash

fzf --ansi --header "\"Select a git repository\"" --reverse --preview "git -C {} branch  --color=always -a" --preview-window "right:30%" --bind "ctrl-]:change-preview-window(70%|30%)" --bind "?:preview:git -C {} log --color=always --graph --all --pretty=format:'%C(auto)%<(30,trunc)%s %C(cyan)%cr %C(auto)%d' " --bind "alt-?:preview:git -C {} branch  --color=always -a "

```

fzfへの引数を追加する場合は、`--fuzzy-finder-args`を使用してください。

```bash
lepos --fuzzy-finder-args "--no-reverse"
```

### preview window

デフォルトでpreview windowを設定しています。\
レポジトリのブランチ一覧を表示します。\
`?`(`shift-/`)でコミットログに切り替えることができます。\
`alt-?`( `shift-alt-/`)で再度ブランチ一覧に切り替えることができます。\
`ctrl-]`でpreview windowのサイズを変更できます。
