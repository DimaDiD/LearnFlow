FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/services/enrollments/LearnFlow.Enrollments.API/LearnFlow.Enrollments.API.csproj", "src/services/enrollments/LearnFlow.Enrollments.API/"]
COPY ["src/services/enrollments/LearnFlow.Enrollments.Application/LearnFlow.Enrollments.Application.csproj", "src/services/enrollments/LearnFlow.Enrollments.Application/"]
COPY ["src/services/enrollments/LearnFlow.Enrollments.Domain/LearnFlow.Enrollments.Domain.csproj", "src/services/enrollments/LearnFlow.Enrollments.Domain/"]
COPY ["src/services/enrollments/LearnFlow.Enrollments.Infrastructure/LearnFlow.Enrollments.Infrastructure.csproj", "src/services/enrollments/LearnFlow.Enrollments.Infrastructure/"]
COPY ["src/shared/LearnFlow.Shared.Contracts/LearnFlow.Shared.Contracts.csproj", "src/shared/LearnFlow.Shared.Contracts/"]
COPY ["src/shared/LearnFlow.Shared.BuildingBlocks/LearnFlow.Shared.BuildingBlocks.csproj", "src/shared/LearnFlow.Shared.BuildingBlocks/"]

RUN dotnet restore "src/services/enrollments/LearnFlow.Enrollments.API/LearnFlow.Enrollments.API.csproj"

COPY . .

RUN dotnet publish "src/services/enrollments/LearnFlow.Enrollments.API/LearnFlow.Enrollments.API.csproj" \
    -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "LearnFlow.Enrollments.API.dll"]