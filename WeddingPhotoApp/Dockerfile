## デバッグ コンテナーをカスタマイズする方法と、Visual Studio がこの Dockerfile を使用してより高速なデバッグのためにイメージをビルドする方法については、https://aka.ms/customizecontainer をご覧ください。
#
## このステージは、VS から高速モードで実行するときに使用されます (デバッグ構成の既定値)
#FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
#USER $APP_UID
#WORKDIR /app
#EXPOSE 8080
#EXPOSE 8081
#
#
## このステージは、サービス プロジェクトのビルドに使用されます
#FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
#ARG BUILD_CONFIGURATION=Release
#WORKDIR /src
#COPY ["WeddingPhotoApp/WeddingPhotoApp.csproj", "WeddingPhotoApp/"]
#RUN dotnet restore "./WeddingPhotoApp/WeddingPhotoApp.csproj"
#COPY . .
#WORKDIR "/src/WeddingPhotoApp"
#RUN dotnet build "./WeddingPhotoApp.csproj" -c $BUILD_CONFIGURATION -o /app/build
#
## このステージは、最終ステージにコピーするサービス プロジェクトを公開するために使用されます
#FROM build AS publish
#ARG BUILD_CONFIGURATION=Release
#RUN dotnet publish "./WeddingPhotoApp.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false
#
## このステージは、運用環境または VS から通常モードで実行している場合に使用されます (デバッグ構成を使用しない場合の既定)
#FROM base AS final
#WORKDIR /app
#COPY --from=publish /app/publish .
#ENTRYPOINT ["dotnet", "WeddingPhotoApp.dll"]

# --- build stage ---
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish -c Release -o /app

# --- runtime stage ---
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine
WORKDIR /app
COPY --from=build /app .

ENV ASPNETCORE_URLS=http://0.0.0.0:${PORT}
ENTRYPOINT ["dotnet", "WeddingPhotoApp.dll"]
