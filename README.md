# JPYC オフライン決済アプリ

このリポジトリには、機能仕様書「JPYC+Authenticator.docx」で説明されているオフライン JPYC 決済システムを実装する **Blazor Hybrid (MAUI)** 製モバイルアプリケーション、Azure Functions バックエンド、Solidity スマートコントラクトが含まれます。

## プロジェクト構成

| パス | 説明 |
|------|------|
| `MobileApp/` | C# MAUI の Blazor Hybrid PWA。Passkey／マイナンバーによる eKYC と Chaum 形式の eCash を BLE/NFC で送受信し、**Travel** と **Merchant** のロールをサポートします。|
| `Backend/OfflineSyncFunction` | バッチ形式の `RedeemBundle` JSON を受け取り、JWT を検証して Cosmos DB に保存し、オンチェーン処理をキューに積む HTTP トリガーの Azure Function。|
| `Backend/RedeemFunction` | `Nethereum` を通じて `RedeemContract.redeem()` を呼び出す耐久オーケストレーションを持つ Azure Function。|
| `SmartContracts/` | Solidity 0.8.x で記述された `OfflineEscrowContract`、`RedeemContract`、`WalletAA`（ERC‑4337 互換）。|
| `docs/` | PlantUML で記載したアーキテクチャ図とシーケンス図。|

## Build

```bash
# .NET 8 と Node 20 が必要
dotnet restore
dotnet build MobileApp/MobileApp.csproj -t:Run
```

### バックエンド

```bash
func start Backend/OfflineSyncFunction
```

### スマートコントラクト

```bash
forge build
```

> 詳細は **docs/architecture.md** を参照してください。
> 初期設定の手順は **docs/azure-setup-ja.md** を参照してください。

## 設定例

Azure Functions を実行する前に、以下の環境変数を設定します:

```bash
COSMOS_CONNECTION="AccountEndpoint=https://<acc>.documents.azure.com:443/;AccountKey=<key>"
BUNDLE_DB_NAME="offline"
BUNDLE_CONTAINER_NAME="bundles"
QUEUE_CONNECTION="DefaultEndpointsProtocol=https;AccountName=<storage>;AccountKey=<key>;EndpointSuffix=core.windows.net"
QUEUE_NAME="redeem-bundles"
JWT_SIGNING_KEY="secret"
USER_CONTAINER_NAME="users"
RPC_URL="https://polygon-rpc.com"
CONTRACT_ADDRESS="0xabcdef1234567890"
PRIVATE_KEY="0x012345..."
```

これらは Cosmos DB、ストレージキューおよび Polygon コントラクトを設定するための値で、`RedeemBundleApi` と `RedeemOrchestrator` が利用します。API キーの取得や変数の設定方法については **docs/jpyc-connection.md** を参照してください。

### ユーザー認証

バックエンドにはアカウント管理用に次のエンドポイントがあります:

- `POST /api/signup` : `{ "username": "alice", "password": "pw" }` の形式で新規ユーザーを作成します。
- `POST /api/signin` : 認証後に `{ "token": "<jwt>" }` を返します。

取得した JWT は `Authorization` ヘッダーに指定して `RedeemBundle` など保護された API を呼び出します。

### Authenticator サービス例

OTP と QR の依存パッケージを追加します:

```xml
<PackageReference Include="Otp.NET" Version="2.0.5" />
<PackageReference Include="ZXing.Net.Maui.Controls" Version="0.4.0" />
```

`MauiProgram.cs` で次のようにサービスを登録します:

```csharp
using ZXing.Net.Maui;
builder.UseBarcodeReader();
builder.Services.AddSingleton<IAuthenticatorService, AuthenticatorService>();
```

TOTP コードを生成する例:

```csharp
var totp = await AuthenticatorService.GenerateTotpAsync("Example", "alice");
```

> 詳細は **docs/architecture.md** を参照してください。

