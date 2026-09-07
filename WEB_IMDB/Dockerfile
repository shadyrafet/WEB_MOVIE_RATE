FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
ARG APP_UID=1654
USER ${APP_UID}
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["WEB_IMDB/WEB_IMDB.csproj", "WEB_IMDB/"]
RUN dotnet restore "WEB_IMDB/WEB_IMDB.csproj"
COPY . .
WORKDIR "/src/WEB_IMDB"
RUN dotnet build "./WEB_IMDB.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./WEB_IMDB.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WEB_IMDB.dll"]
