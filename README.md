# JPYC Offline Payment App

This repository contains a **Blazor Hybrid (MAUI)** mobile application, Azure Functions backend, and Solidity smart‑contracts implementing an offline JPYC payment system described in the functional specification _JPYC+Authenticator.docx_.

## Projects

| Path | Description |
|------|-------------|
| `MobileApp/` | C# MAUI Blazor Hybrid PWA supporting **Travel** & **Merchant** roles with Passkey / My‑Number eKYC and Chaum‑style eCash transfer over BLE/NFC. |
| `Backend/OfflineSyncFunction` | Azure Function (HTTP trigger) that receives batch `RedeemBundle` JSON, validates JWT, stores in Cosmos DB, and queues on‑chain redemption. |
| `Backend/RedeemFunction` | Azure Function with durable orchestration calling `RedeemContract.redeem()` via `Nethereum`. |
| `SmartContracts/` | Solidity 0.8.x contracts: `OfflineEscrowContract`, `RedeemContract`, `WalletAA` (ERC‑4337 compatible). |
| `docs/` | Architecture diagrams and sequence charts (PlantUML). |

## Build

```bash
# .NET 8 & Node 20 required
dotnet restore
dotnet build MobileApp/MobileApp.csproj -t:Run
```

Back‑end:

```bash
func start Backend/OfflineSyncFunction
```

Contracts:

```bash
forge build
```

### Authenticator Service Sample

Add OTP and QR dependencies:

```xml
<PackageReference Include="Otp.NET" Version="2.0.5" />
<PackageReference Include="ZXing.Net.Maui.Controls" Version="0.4.0" />
```

In `MauiProgram.cs` register the service:

```csharp
using ZXing.Net.Maui;
builder.UseBarcodeReader();
builder.Services.AddSingleton<IAuthenticatorService, AuthenticatorService>();
```

Generate a TOTP code:

```csharp
var totp = AuthenticatorService.GenerateTotp("Example", "alice");
```

> See **docs/architecture.md** for more detail.
