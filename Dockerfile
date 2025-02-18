FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["src/pagamentos.Api/pagamentos.Api.csproj", "pagamentos.Api/"]
COPY ["src/pagamentos.Domain/pagamentos.Domain.csproj", "pagamentos.Domain/"]
COPY ["src/pagamentos.Apps/pagamentos.Apps.csproj", "pagamentos.Apps/"]
COPY ["src/pagamentos.Adapters/pagamentos.Adapters.csproj", "pagamentos.Adapters/"]
COPY ["src/pagamentos.Infrastructure/pagamentos.Infrastructure.csproj", "pagamentos.Infrastructure/"]

RUN dotnet restore

COPY . .
WORKDIR "/src/pagamentos.Api"

RUN dotnet build "pagamentos.Api.csproj" -c $BUILD_CONFIGURATION --no-dependencies -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "pagamentos.Api.csproj" -c $BUILD_CONFIGURATION --no-dependencies -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "pagamentos.Api.dll"]

