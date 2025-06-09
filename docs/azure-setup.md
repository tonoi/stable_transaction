# Azure クラウド接続と秘密情報設定手順

このドキュメントでは、モバイルアプリを今回の Azure Functions バックエンドに接続する方法と、JPYC のトランザクションを実行するための秘密情報を取得して設定する手順を解説します。

## モバイルアプリを Azure Functions に接続する
1. **Azure Portal** にサインインし、デプロイした関数アプリの画面を開きます。
2. サイドバーから **Functions** を選択し、任意の関数を開いて **Get Function URL** をクリックします。`https://<app>.azurewebsites.net/api/` のようなベース URL が確認できます。
3. リポジトリ内 `MobileApp/Services/UserService.cs` を開き、`BaseAddress` フィールドを取得した URL に書き換えます。末尾のスラッシュも含めて設定してください。
4. ファイルを保存したら以下のコマンドでアプリを再ビルドします。
   ```bash
   dotnet build MobileApp/MobileApp.csproj
   ```
5. 生成されたアプリをエミュレーターや実機にデプロイして、Azure Functions にアクセスできることを確認します。

## JPYC トランザクション用シークレットの準備と設定
1. Polygon 用の RPC エンドポイントを契約して取得します。Alchemy や Infura 等のサービスから発行される URL を `RPC_URL` として控えます。
2. `RedeemContract` を Polygon ネットワーク上にデプロイし、そのアドレスを `CONTRACT_ADDRESS` としてメモします。
3. トランザクション送信に利用するウォレットの秘密鍵を `PRIVATE_KEY` として準備します。第三者には絶対に共有しないよう注意してください。
4. Azure Portal で対象の関数アプリの **Configuration** > **Application settings** を開きます。
5. **New application setting** を選択し、次のキーと値をそれぞれ追加します。
   - `RPC_URL` – 手順 1 で取得した URL
   - `CONTRACT_ADDRESS` – 手順 2 のアドレス
   - `PRIVATE_KEY` – 手順 3 の秘密鍵
   必要に応じて `COSMOS_CONNECTION` など他の接続文字列も同じ画面で設定します。
6. 画面上部の **Save** を押して変更を反映し、ポップアップが表示されたら **Continue** を選択して関数アプリを再起動します。
7. 再起動後、Azure Portal の **Log stream** でアプリが起動し、設定が読み込まれているか確認してください。

以上の手順を実施すると、モバイルアプリはクラウド上のバックエンドと接続し、実際の JPYC トランザクションを処理できるようになります。
