# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Directory.Build.props .
COPY Directory.Packages.props .
COPY src/Degen.Domain/Degen.Domain.csproj src/Degen.Domain/
COPY src/Degen.Application/Degen.Application.csproj src/Degen.Application/
COPY src/Degen.Infrastructure/Degen.Infrastructure.csproj src/Degen.Infrastructure/
COPY src/Degen.Contracts/Degen.Contracts.csproj src/Degen.Contracts/
COPY src/Degen.Api/Degen.Api.csproj src/Degen.Api/

RUN dotnet restore src/Degen.Api

COPY . .
RUN dotnet publish src/Degen.Api/Degen.Api.csproj \
    -c Release \
    -o /out \
    /p:UseAppHost=false \
    /p:DebugSymbols=false \
    /p:DebugType=None

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /out .

USER app

EXPOSE 8080

ENTRYPOINT ["dotnet", "Degen.Api.dll"]
