# Azure 初期設定ガイド

GitHub Actions から Azure Functions へデプロイするための基本手順をまとめます。複雑な操作を極力省いた簡易版です。

## 1. リソースグループを作成する
Azure Portal または `az` CLI でデプロイ先となるリソースグループを作成します。
```bash
az group create --name my-resource-group --location japaneast
```

## 2. ストレージアカウントを準備する
Functions ランタイムとキュー用に 1 つストレージアカウントを作成します。キュー名 `redeem-bundles` を追加し、接続文字列を控えておきます。

## 3. Cosmos DB を準備する
Cosmos DB アカウントを作成し、データベース `offline`、コンテナ `bundles` と `users` を用意します。こちらも接続文字列をメモします。

## 4. Function App を 2 つ作成する
- `OfflineSyncFunction` 用
- `RedeemFunction` 用

いずれも .NET 8 Isolated ワーカーを選択し、発行プロファイルをダウンロードしておきます。

## 5. アプリケーション設定を追加する
各 Function App の "アプリケーション設定" に以下を登録します。値の例は `README.md` の `Configuration Sample` を参照してください。
- `COSMOS_CONNECTION`
- `BUNDLE_DB_NAME`（例: `offline`）
- `BUNDLE_CONTAINER_NAME`（例: `bundles`）
- `QUEUE_CONNECTION`
- `QUEUE_NAME`（例: `redeem-bundles`）
- `JWT_SIGNING_KEY`
- `USER_CONTAINER_NAME` など

## 6. GitHub Secrets を登録する
リポジトリの *Settings > Secrets* で次を追加します。
- `OFFLINE_FUNCTIONAPP_NAME` と `OFFLINE_FUNCTIONAPP_PUBLISH_PROFILE`
- `REDEEM_FUNCTIONAPP_NAME` と `REDEEM_FUNCTIONAPP_PUBLISH_PROFILE`

## 7. ワークフローを実行する
main ブランチへ push するか、Actions タブから手動実行すると、 `.github/workflows/azure-deploy.yml` が Azure Functions を自動的にビルド・デプロイします。

以上で初期設定は完了です。
