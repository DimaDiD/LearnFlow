FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/services/certificates/LearnFlow.Certificates.API/LearnFlow.Certificates.API.csproj", "src/services/certificates/LearnFlow.Certificates.API/"]
COPY ["src/services/certificates/LearnFlow.Certificates.Application/LearnFlow.Certificates.Application.csproj", "src/services/certificates/LearnFlow.Certificates.Application/"]
COPY ["src/services/certificates/LearnFlow.Certificates.Domain/LearnFlow.Certificates.Domain.csproj", "src/services/certificates/LearnFlow.Certificates.Domain/"]
COPY ["src/services/certificates/LearnFlow.Certificates.Infrastructure/LearnFlow.Certificates.Infrastructure.csproj", "src/services/certificates/LearnFlow.Certificates.Infrastructure/"]
COPY ["src/shared/LearnFlow.Shared.Contracts/LearnFlow.Shared.Contracts.csproj", "src/shared/LearnFlow.Shared.Contracts/"]
COPY ["src/shared/LearnFlow.Shared.BuildingBlocks/LearnFlow.Shared.BuildingBlocks.csproj", "src/shared/LearnFlow.Shared.BuildingBlocks/"]

RUN dotnet restore "src/services/certificates/LearnFlow.Certificates.API/LearnFlow.Certificates.API.csproj"

COPY . .

RUN dotnet publish "src/services/certificates/LearnFlow.Certificates.API/LearnFlow.Certificates.API.csproj" \
    -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "LearnFlow.Certificates.API.dll"]