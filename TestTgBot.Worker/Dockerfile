FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
USER $APP_UID
WORKDIR /app


FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["TestTgBot.Worker/TestTgBot.Worker.csproj", "TestTgBot.Worker/"]
COPY ["TestTgBot.Application/TestTgBot.Application.csproj", "TestTgBot.Application/"]
RUN dotnet restore "./TestTgBot.Worker/TestTgBot.Worker.csproj"
COPY . .
WORKDIR "/src/TestTgBot.Worker"
RUN dotnet build "./TestTgBot.Worker.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./TestTgBot.Worker.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TestTgBot.Worker.dll"]