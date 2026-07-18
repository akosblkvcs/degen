FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Restore against the project files alone so this layer is cached
# until a package reference actually changes.
COPY Degen.slnx ./
COPY src/Degen.Domain/Degen.Domain.csproj src/Degen.Domain/
COPY src/Degen.Infrastructure/Degen.Infrastructure.csproj src/Degen.Infrastructure/
COPY src/Degen.Api/Degen.Api.csproj src/Degen.Api/
RUN dotnet restore Degen.slnx

COPY . .
RUN dotnet publish src/Degen.Api -c Release --no-restore -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .
USER $APP_UID
EXPOSE 8080
ENTRYPOINT ["dotnet", "Degen.Api.dll"]
