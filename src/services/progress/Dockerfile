FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/services/progress/LearnFlow.Progress.API/LearnFlow.Progress.API.csproj", "src/services/progress/LearnFlow.Progress.API/"]
COPY ["src/services/progress/LearnFlow.Progress.Application/LearnFlow.Progress.Application.csproj", "src/services/progress/LearnFlow.Progress.Application/"]
COPY ["src/services/progress/LearnFlow.Progress.Domain/LearnFlow.Progress.Domain.csproj", "src/services/progress/LearnFlow.Progress.Domain/"]
COPY ["src/services/progress/LearnFlow.Progress.Infrastructure/LearnFlow.Progress.Infrastructure.csproj", "src/services/progress/LearnFlow.Progress.Infrastructure/"]
COPY ["src/shared/LearnFlow.Shared.Contracts/LearnFlow.Shared.Contracts.csproj", "src/shared/LearnFlow.Shared.Contracts/"]
COPY ["src/shared/LearnFlow.Shared.BuildingBlocks/LearnFlow.Shared.BuildingBlocks.csproj", "src/shared/LearnFlow.Shared.BuildingBlocks/"]

RUN dotnet restore "src/services/progress/LearnFlow.Progress.API/LearnFlow.Progress.API.csproj"

COPY . .

RUN dotnet publish "src/services/progress/LearnFlow.Progress.API/LearnFlow.Progress.API.csproj" \
    -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "LearnFlow.Progress.API.dll"]