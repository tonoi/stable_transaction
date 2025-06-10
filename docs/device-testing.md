# Device Testing Guide

## Android Studio でビルド
1. .NET 8 SDK と MAUI Android ワークロードをインストールします。
   ```bash
   dotnet workload install maui-android
   ```
2. Android Studio を起動し、**Open an Existing Project** で `MobileApp` ディレクトリを選択します。
3. `Run` ボタンを押すと、接続済みデバイスまたはエミュレーター上でアプリがビルドされます。
   CLI から実行する場合は次を使用します。
   ```bash
   dotnet build MobileApp/MobileApp.csproj -t:Run -f net8.0-android
   ```

## Xcode でビルド
1. macOS 上で .NET 8 SDK と MAUI iOS ワークロードをインストールします。
   ```bash
   dotnet workload install maui-ios
   ```
2. ターミナルで iOS ビルドを生成します。
   ```bash
   dotnet build MobileApp/MobileApp.csproj -f net8.0-ios
   ```
3. 生成された `bin/Debug/net8.0-ios` 内の `.app` を Xcode の Simulator で開きます。
   ```bash
   open bin/Debug/net8.0-ios/iossimulator/MobileApp.app
   ```

## 生体認証のテスト
1. アプリを起動し、`/bioauth` ページを開きます。
2. **Authenticate** ボタンをタップすると `IAuthenticatorService` が呼び出されます。
3. Android エミュレーターでは次のコマンドで指紋を送信できます。
   ```bash
   adb -e emu finger touch 1
   ```
4. iOS Simulator ではメニューの `Hardware > Touch ID > Enrolled` を選択し、指紋認証をシミュレートします。

## Passkey の設定
1. OS の設定からパスキー(デバイス認証情報)を登録します。
   - Android: 設定 > パスワードとアカウント > パスキー
   - iOS: 設定 > パスワード > パスキー
2. アプリ内のログイン画面で **Continue with Passkey** を選択し、登録した認証情報を使用します。

## スクリーンショット取得
- Android デバイス
  ```bash
  adb exec-out screencap -p > screen.png
  ```
- iOS Simulator
  ```bash
  xcrun simctl io booted screenshot screen.png
  ```


