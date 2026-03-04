# ── Build stage ──────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj files first for layer caching
COPY Directory.Build.props .
COPY Directory.Packages.props .
COPY src/Degen.Domain/Degen.Domain.csproj src/Degen.Domain/
COPY src/Degen.Application/Degen.Application.csproj src/Degen.Application/
COPY src/Degen.Infrastructure/Degen.Infrastructure.csproj src/Degen.Infrastructure/
COPY src/Degen.Contracts/Degen.Contracts.csproj src/Degen.Contracts/
COPY src/Degen.Api/Degen.Api.csproj src/Degen.Api/

RUN dotnet restore src/Degen.Api

# Copy everything and publish
COPY . .
RUN dotnet publish src/Degen.Api -c Release -o /app

# ── Runtime stage ────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app .

USER app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Degen.Api.dll"]
