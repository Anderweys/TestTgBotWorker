FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["TestTgBotWorker/TestTgBotWorker.csproj", "TestTgBotWorker/"]
RUN dotnet restore "TestTgBotWorker/TestTgBotWorker.csproj"
COPY . .
WORKDIR "/src/TestTgBotWorker"
RUN dotnet build "TestTgBotWorker.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "TestTgBotWorker.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "TestTgBotWorker.dll"]