# AgencyVTuberDBManager

vindies.jp の VTuberフィルタリング用データベース(所属/独立判定データ)を管理するための、社内利用限定のWPFデスクトップアプリ。

## 背景

vindies本体(Webアプリ)側にDB編集用のページを用意する案もあったが、以下の理由からリスクが大きいと判断し、別アプリとして切り出した。

- Web経由での編集画面公開は、意図しないアクセスや脆弱性のリスクを増やす
- 管理者以外が触る必要のない機能をWebアプリのアタックサーフェスに含めたくない

そのため、DB編集はWPFのデスクトップアプリに閉じ、vindies APIに自作のAPIキーを付与してリクエストする方式にした。DBへ直接接続はせず、必ずvindies API経由でアクセスする。

## 主な機能

- 事務所所属VTuberデータのCRUD

## 技術スタック

- WPF (.NET)
- PostgreSQL(vindies API経由でアクセス、直接接続なし)
- 認証: 自作APIキーをvindies APIへのリクエストに付与

## アーキテクチャ

MVVM + DIパターンを採用。

### Window管理

DIパターン導入時、Windowの親子関係の紐づけで詰まった。よく使われる

```csharp
Application.Current.Windows
    .OfType<TWindow>()
    .FirstOrDefault();
```

という方法は、同じ型のWindowが複数開いている場合にどのインスタンスが返るか保証がなく、意図したWindowを取得できない可能性がある。

この問題を避けるため、Window間の親子関係をDictionaryで管理する自作の仕組みを実装した。DIコンテナから解決したWindowインスタンスをキーに紐づけて管理することで、複数インスタンスが存在する状況でも意図した親子関係を確実に辿れるようにしている。

## セットアップ

以下のパスにTwitch APIキーとvindies APIキーを記載した `appsettings.json` を用意する。

```
.\VtuberDbMgr\VtuberDbManager\VtuberDbManager\appsettings.json
```
