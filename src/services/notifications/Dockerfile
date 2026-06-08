FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/services/notifications/LearnFlow.Notifications.API/LearnFlow.Notifications.API.csproj", "src/services/notifications/LearnFlow.Notifications.API/"]
COPY ["src/services/notifications/LearnFlow.Notifications.Application/LearnFlow.Notifications.Application.csproj", "src/services/notifications/LearnFlow.Notifications.Application/"]
COPY ["src/services/notifications/LearnFlow.Notifications.Domain/LearnFlow.Notifications.Domain.csproj", "src/services/notifications/LearnFlow.Notifications.Domain/"]
COPY ["src/services/notifications/LearnFlow.Notifications.Infrastructure/LearnFlow.Notifications.Infrastructure.csproj", "src/services/notifications/LearnFlow.Notifications.Infrastructure/"]
COPY ["src/shared/LearnFlow.Shared.Contracts/LearnFlow.Shared.Contracts.csproj", "src/shared/LearnFlow.Shared.Contracts/"]
COPY ["src/shared/LearnFlow.Shared.BuildingBlocks/LearnFlow.Shared.BuildingBlocks.csproj", "src/shared/LearnFlow.Shared.BuildingBlocks/"]

RUN dotnet restore "src/services/notifications/LearnFlow.Notifications.API/LearnFlow.Notifications.API.csproj"

COPY . .

RUN dotnet publish "src/services/notifications/LearnFlow.Notifications.API/LearnFlow.Notifications.API.csproj" \
    -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "LearnFlow.Notifications.API.dll"]