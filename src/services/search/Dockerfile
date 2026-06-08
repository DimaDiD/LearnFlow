FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/services/search/LearnFlow.Search.API/LearnFlow.Search.API.csproj", "src/services/search/LearnFlow.Search.API/"]
COPY ["src/services/search/LearnFlow.Search.Application/LearnFlow.Search.Application.csproj", "src/services/search/LearnFlow.Search.Application/"]
COPY ["src/services/search/LearnFlow.Search.Domain/LearnFlow.Search.Domain.csproj", "src/services/search/LearnFlow.Search.Domain/"]
COPY ["src/services/search/LearnFlow.Search.Infrastructure/LearnFlow.Search.Infrastructure.csproj", "src/services/search/LearnFlow.Search.Infrastructure/"]
COPY ["src/shared/LearnFlow.Shared.Contracts/LearnFlow.Shared.Contracts.csproj", "src/shared/LearnFlow.Shared.Contracts/"]
COPY ["src/shared/LearnFlow.Shared.BuildingBlocks/LearnFlow.Shared.BuildingBlocks.csproj", "src/shared/LearnFlow.Shared.BuildingBlocks/"]

RUN dotnet restore "src/services/search/LearnFlow.Search.API/LearnFlow.Search.API.csproj"

COPY . .

RUN dotnet publish "src/services/search/LearnFlow.Search.API/LearnFlow.Search.API.csproj" \
    -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "LearnFlow.Search.API.dll"]