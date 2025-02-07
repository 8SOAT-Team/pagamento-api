FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Pagamento.Api/Pagamento.Api.csproj", "Pagamento.Api/"]
COPY ["Pagamento.Domain/Pagamento.Domain.csproj", "Pagamento.Domain/"]
COPY ["Pagamento.Apps/Pagamento.Apps.csproj", "Pagamento.Apps/"]
COPY ["Pagamento.Adapters/Pagamento.Adapters.csproj", "Pagamento.Adapters/"]
COPY ["Pagamento.Infrastructure/Pagamento.Infrastructure.csproj", "Pagamento.Infrastructure/"]
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
