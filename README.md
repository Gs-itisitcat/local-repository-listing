# git-local-repository-listing
List git local repository on your computer.

## 実装予定コマンド

### Sub command

- search: ディレクトリ内のリポジトリ検索、およびlist保存
  - args: 検索するディレクトリパス
  - --all/-a: 全ドライブのルートから再帰的に検索
  - --recusive/-r: 再帰的に検索
  - --silent/-s: 検索結果を表示しない
- list: 保存されたリポジトリを表示
  - args: リポジトリの名前でフィルタリング
  - --verbose/-v: 詳細な情報表示
  - --update/-u: search --allをしてから表示
- switch/cd: リポジトリに移動
  - args: 移動先リポジトリ名
  - --update/-u: serach --allをしてから移動
  - --interactive/-i: ダイアログに従って指定
