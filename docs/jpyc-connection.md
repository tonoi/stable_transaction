# JPYC 接続設定ガイド

このプロジェクトでは、JPYC トークンを扱うスマートコントラクトとやり取りするために
Polygon ネットワーク上の RPC エンドポイントとウォレットの秘密鍵が必要です。
ここでは API Key / シークレットの取得方法と環境変数への設定手順を説明します。

## 1. RPC エンドポイントの取得
1. Infura、Alchemy、QuickNode などの RPC プロバイダーに登録します。
2. 新しいプロジェクトを作成し、Polygon (Mainnet または Testnet) 用の RPC URL を取得します。
   この URL にはプロバイダー発行の API Key が含まれます。
3. 取得した URL を `RPC_URL` 環境変数に設定します。

```bash
export RPC_URL="https://polygon-mainnet.g.alchemy.com/v2/xxxxxxxxxxxxxxxx"
```

## 2. ウォレットの秘密鍵
1. MetaMask などのウォレットで使用するアカウントを準備します。
2. アカウントの秘密鍵をエクスポートし、`PRIVATE_KEY` 環境変数に設定します。
   秘密鍵は外部に漏れないよう厳重に管理してください。

```bash
export PRIVATE_KEY="0x0123456789abcdef..."
```

## 3. スマートコントラクトのアドレス
JPYC 用にデプロイした `RedeemContract` のアドレスを `CONTRACT_ADDRESS` 環境変数に設定します。

```bash
export CONTRACT_ADDRESS="0xabcdef1234567890abcdef1234567890abcdef12"
```

## 4. その他の環境変数
README の `Configuration Sample` で示されている Cosmos DB や
ストレージキューの接続文字列も同様に設定します。

```bash
export COSMOS_CONNECTION="AccountEndpoint=https://<acc>.documents.azure.com:443/;AccountKey=<key>"
export QUEUE_CONNECTION="DefaultEndpointsProtocol=https;AccountName=<storage>;AccountKey=<key>;EndpointSuffix=core.windows.net"
```

## 5. Azure Functions での読み込み
Azure Functions では、上記の環境変数を利用して `RedeemBundleApi` と
`RedeemOrchestrator` が Polygon ネットワークへアクセスします。
設定後に `func start` で Functions を起動すると接続が有効になります。

## 6. セキュリティ上の注意
- 秘密鍵や API Key をソースコード管理に含めないよう `.env` などに保存し、 `.gitignore` に追加してください。
- クラウド環境へデプロイする際は適切なアクセス制御を行い、権限を最小限に保ちます。
