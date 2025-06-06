# Architecture Overview

```
+-------------------+      BLE/NFC/QR      +-------------------+
|  Traveler Wallet  |  <---------------->  |   Merchant POS    |
|  Blazor Hybrid PWA|                     | Blazor Hybrid PWA |
+---------+---------+                     +---------+---------+
          |  Sync (online)                           |
          v                                          v
+-----------------------+   HTTP/gRPC   +-----------------------+
| OfflineSync Function  | ------------> |  Cosmos DB (buffer)   |
+-----------------------+               +-----------------------+
          | Durable Orchestrator
          v
+-----------------------+   JSON-RPC   +-----------------------+
| RedeemFunction (web3) | -----------> |  Polygon L2 Contracts |
+-----------------------+              +-----------------------+
```

Sequence diagrams and detailed data models are available in the Word specification and Appendix.