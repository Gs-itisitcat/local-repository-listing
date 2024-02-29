# git-local-repository-listing

List git local repository on your computer.

ローカルにあるgitリポジトリを検索し、fazzy finderで選択して移動するためのツール

## 実装予定コマンド

- 引数無し: 全ドライブのルートから再帰的に検索
- 引数有り: 全ドライブのルートから再帰的に検索し、引数で指定したディレクトリ名を含むリポジトリをクエリ

### flags

- --root/-r: 検索するディレクトリパス
- --non-recursive/-n: 再帰的に検索しない
- --list-only/-l: 検索結果の出力のみ
- --exclude-path/-e: 検索対象から除外するディレクトリパス (複数指定可)
- --exclude-name/-E: 検索対象から除外するディレクトリ名 (複数指定可)
- --search [options]: optionで指定した文字列を含むリポジトリを検索 (複数指定可)
  - name: リポジトリ名
  - path: リポジトリのパス (デフォルト) (pathとfull-pathは排他)
  - full-path: リポジトリのフルパス (--allの場合pathと同じ) (pathとfull-pathは排他)
  - url: リポジトリのURL
