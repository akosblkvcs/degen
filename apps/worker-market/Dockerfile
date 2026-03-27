# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY apps/api-core/Directory.Build.props apps/api-core/
COPY apps/api-core/Directory.Packages.props apps/api-core/
COPY apps/api-core/src/Degen.Domain/Degen.Domain.csproj apps/api-core/src/Degen.Domain/
COPY apps/api-core/src/Degen.Contracts/Degen.Contracts.csproj apps/api-core/src/Degen.Contracts/
COPY apps/api-core/src/Degen.Application/Degen.Application.csproj apps/api-core/src/Degen.Application/
COPY apps/api-core/src/Degen.Infrastructure/Degen.Infrastructure.csproj apps/api-core/src/Degen.Infrastructure/

COPY apps/worker-market/Directory.Build.props apps/worker-market/
COPY apps/worker-market/Directory.Packages.props apps/worker-market/
COPY apps/worker-market/global.json apps/worker-market/
COPY apps/worker-market/src/Degen.Worker.Market/Degen.Worker.Market.csproj apps/worker-market/src/Degen.Worker.Market/

RUN dotnet restore apps/worker-market/src/Degen.Worker.Market/Degen.Worker.Market.csproj

COPY apps/api-core/src/ apps/api-core/src/
COPY apps/worker-market/ apps/worker-market/
RUN dotnet publish apps/worker-market/src/Degen.Worker.Market/Degen.Worker.Market.csproj -c Release -o /app --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/runtime:10.0 AS runtime
WORKDIR /app

COPY --from=build /out .

USER app

ENTRYPOINT ["dotnet", "Degen.Worker.Market.dll"]
