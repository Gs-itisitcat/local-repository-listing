# git-local-repository-listing

JA | [EN](README-en.md)

List git local repository on your computer.

ローカルにあるgitリポジトリを検索し、fazzy finderで選択して移動するツール

## インストール

[Release](https://github.com/Gs-itisitcat/git-local-repository-listing/releases)からダウンロードして、任意のディレクトリに配置してください。
lepos.bashを.bashrcなどにsourceしてください。

```bash
source /path/to/lepos.bash
```

## コマンド

```bash
lepos [options] [query]
```

- 引数無し: 全ドライブのルートから再帰的に検索
- 引数有り: 全ドライブのルートから再帰的に検索し、引数で指定したディレクトリ名を含むリポジトリをクエリ

### flags

- --root/-r: 検索するディレクトリパス
- --non-recursive/-n: 再帰的に検索しない
- --list-only/-l: 検索結果の出力のみ
- --exclude-name/-e: 検索対象から除外するディレクトリ名 (複数指定可)
- --exclude-path/-ep: 検索対象から除外するディレクトリパス (複数指定可)
- --fuzzy-finder-args/-a: fzfに渡す引数 (複数指定可)
