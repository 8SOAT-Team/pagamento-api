FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["src/Pagamentos.Api/Pagamentos.Api.csproj", "src/Pagamentos.Api/"]
COPY ["src/Pagamentos.Domain/Pagamentos.Domain.csproj", "src/Pagamentos.Domain/"]
COPY ["src/Pagamentos.Apps/Pagamentos.Apps.csproj", "src/Pagamentos.Apps/"]
COPY ["src/Pagamentos.Adapters/Pagamentos.Adapters.csproj", "src/Pagamentos.Adapters/"]
COPY ["src/Pagamentos.Infrastructure/Pagamentos.Infrastructure.csproj", "src/Pagamentos.Infrastructure/"]

RUN dotnet restore "./src/Pagamentos.Api/Pagamentos.Api.csproj"

COPY . .
WORKDIR "/src/src/Pagamentos.Api"
RUN dotnet build "Pagamentos.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Pagamentos.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Pagamentos.Api.dll"]
