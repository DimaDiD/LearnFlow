FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/services/identity/LearnFlow.Identity.API/LearnFlow.Identity.API.csproj", "src/services/identity/LearnFlow.Identity.API/"]
COPY ["src/services/identity/LearnFlow.Identity.Application/LearnFlow.Identity.Application.csproj", "src/services/identity/LearnFlow.Identity.Application/"]
COPY ["src/services/identity/LearnFlow.Identity.Domain/LearnFlow.Identity.Domain.csproj", "src/services/identity/LearnFlow.Identity.Domain/"]
COPY ["src/services/identity/LearnFlow.Identity.Infrastructure/LearnFlow.Identity.Infrastructure.csproj", "src/services/identity/LearnFlow.Identity.Infrastructure/"]
COPY ["src/shared/LearnFlow.Shared.Contracts/LearnFlow.Shared.Contracts.csproj", "src/shared/LearnFlow.Shared.Contracts/"]
COPY ["src/shared/LearnFlow.Shared.BuildingBlocks/LearnFlow.Shared.BuildingBlocks.csproj", "src/shared/LearnFlow.Shared.BuildingBlocks/"]

RUN dotnet restore "src/services/identity/LearnFlow.Identity.API/LearnFlow.Identity.API.csproj"

COPY . .

RUN dotnet publish "src/services/identity/LearnFlow.Identity.API/LearnFlow.Identity.API.csproj" \
    -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "LearnFlow.Identity.API.dll"]