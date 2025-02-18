FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["src/Pagamento.Domain/Pagamento.Domain.csproj", "src/Pagamento.Domain/"]
COPY ["src/Pagamento.Infrastructure/Pagamento.Infrastructure.csproj", "src/Pagamento.Infrastructure/"]
COPY ["src/Pagamento.Adapters/Pagamento.Adapters.csproj", "src/Pagamento.Adapters/"]
COPY ["src/Pagamento.Apps/Pagamento.Apps.csproj", "src/Pagamento.Apps/"]
COPY ["src/Pagamento.Api/Pagamento.Api.csproj", "src/Pagamento.Api/"]

RUN dotnet restore "./src/Pagamento.Api/Pagamento.Api.csproj"

COPY . .

WORKDIR "/src/src/Pagamento.Api"
RUN dotnet build "./Pagamento.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Pagamento.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS finalcd
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Pagamento.Api.dll"]
