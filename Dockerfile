lsFROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["DocumentManagerApi/DocumentManagerApi.csproj", "DocumentManagerApi/"]
COPY ["DocumentManagerModel/DocumentManagerModel.csproj", "DocumentManagerModel/"]
COPY ["DocumentManager.TextExtractor/DocumentManager.TextExtractor.csproj", "DocumentManager.TextExtractor/"]
COPY ["DocumentManagerPersistence/DocumentManagerPersistence.csproj", "DocumentManagerPersistence/"]
COPY ["DocumentManager.DocumentProcessor/DocumentManager.DocumentProcessor.csproj", "DocumentManager.DocumentProcessor/"]
RUN dotnet restore "DocumentManagerApi/DocumentManagerApi.csproj"
COPY . .
WORKDIR "/src/DocumentManagerApi"
RUN dotnet build "DocumentManagerApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DocumentManagerApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DocumentManagerApi.dll"]
