# JobFlow

Reliable Distributed Job Processing Platform.

## Solution

Clean Architecture on ASP.NET Core (.NET 10):

```
JobFlow.slnx
├── src
│   ├── JobFlow.API             # HTTP API (composition root)
│   ├── JobFlow.Application     # Use cases, ports
│   ├── JobFlow.Domain          # Entities and domain rules
│   ├── JobFlow.Infrastructure  # EF Core, RabbitMQ, Redis
│   └── JobFlow.Worker          # Background job consumers
└── tests
    ├── JobFlow.UnitTests
    └── JobFlow.IntegrationTests
```

Dependencies flow inward:

`API / Worker → Application → Domain`  
`Infrastructure → Application` (implements interfaces)

## Run

```bash
dotnet run --project src/JobFlow.API
dotnet run --project src/JobFlow.Worker
```
