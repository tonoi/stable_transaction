# オフライン決済とクラウド同期のフロー

このドキュメントでは、旅行者端末と加盟店端末がネットワークに接続されていない状態で決済を行う手順と、各端末がオンラインに戻った際にクラウドへ取引情報を同期する流れを示します。

## 1. 端末間のオフライン決済

```mermaid
sequenceDiagram
    participant T as Traveler Wallet
    participant M as Merchant POS

    T->>M: トークンバンドル送信(BLE/NFC)
    M->>M: トークンをローカル保存
    M-->>T: 受領確認
```

## 2. オンライン同期

```mermaid
sequenceDiagram
    participant T as Traveler Wallet
    participant M as Merchant POS
    participant F as OfflineSync Function
    participant DB as Cosmos DB
    participant R as RedeemFunction
    participant C as Polygon Contract

    %% 加盟店がオンラインになった場合
    M->>F: POST RedeemBundle
    F->>DB: バンドル保存
    F->>R: オンチェーン処理をキュー
    R->>C: redeemBundle()

    %% 旅行者がオンラインになった場合
    T->>F: POST UsedSerials
    F->>DB: シリアルを消費済みに更新
```

クラウド側でバンドルの検証とチェーンへの送信が完了すると、加盟店はオンチェーンでトークンを受け取ります。旅行者端末では消費済みトークンが記録され、二重支払いを防止します。

