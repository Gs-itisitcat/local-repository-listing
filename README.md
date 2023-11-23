# git-local-repository-listing

List git local repository on your computer.

ローカルにあるgitリポジトリを検索し、fazzy finderで選択して移動するためのツール

## 実装予定コマンド

- 引数無し: 全ドライブのルートから再帰的に検索
- 引数有り: 全ドライブのルートから再帰的に検索し、引数で指定したディレクトリ名を含むリポジトリをクエリ

### flags

- --all/-a: 全ドライブのルートから再帰的に検索 (デフォルト)
- --path/-p: 検索するディレクトリパス
- --recusive/-r: 再帰的に検索 (デフォルト)
- --no-recusive/-R: 再帰的に検索しない
- --list-only/-l: 検索結果の出力のみ
- --verbose/-v: 詳細な情報表示 (--list-onlyの場合のみ有効)
- --search [options]: optionで指定した文字列を含むリポジトリを検索 (複数指定可)
  - name: リポジトリ名
  - path: リポジトリのパス (デフォルト) (pathとfullpathは排他)
  - fullpath: リポジトリのフルパス (--allの場合pathと同じ) (pathとfullpathは排他)
  - url: リポジトリのURL
