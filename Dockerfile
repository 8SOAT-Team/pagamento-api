FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["src/Pagamentos.Api/Pagamentos.Api.csproj", "pagamentos.Api/"]
COPY ["src/Pagamentos.Domain/Pagamentos.Domain.csproj", "pagamentos.Domain/"]
COPY ["src/Pagamentos.Apps/Pagamentos.Apps.csproj", "pagamentos.Apps/"]
COPY ["src/Pagamentos.Adapters/Pagamentos.Adapters.csproj", "pagamentos.Adapters/"]
COPY ["src/Pagamentos.Infrastructure/Pagamentos.Infrastructure.csproj", "Pagamentos.Infrastructure/"]

RUN dotnet restore "Pagamento.Api/Pagamento.Api.csproj"

COPY . .
WORKDIR "/src/Pagamento.Api"
RUN dotnet build "Pagamento.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Pagamento.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Pagamento.Api.dll"]