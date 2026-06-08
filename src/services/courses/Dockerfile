FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/services/courses/LearnFlow.Courses.API/LearnFlow.Courses.API.csproj", "src/services/courses/LearnFlow.Courses.API/"]
COPY ["src/services/courses/LearnFlow.Courses.Application/LearnFlow.Courses.Application.csproj", "src/services/courses/LearnFlow.Courses.Application/"]
COPY ["src/services/courses/LearnFlow.Courses.Domain/LearnFlow.Courses.Domain.csproj", "src/services/courses/LearnFlow.Courses.Domain/"]
COPY ["src/services/courses/LearnFlow.Courses.Infrastructure/LearnFlow.Courses.Infrastructure.csproj", "src/services/courses/LearnFlow.Courses.Infrastructure/"]
COPY ["src/shared/LearnFlow.Shared.Contracts/LearnFlow.Shared.Contracts.csproj", "src/shared/LearnFlow.Shared.Contracts/"]
COPY ["src/shared/LearnFlow.Shared.BuildingBlocks/LearnFlow.Shared.BuildingBlocks.csproj", "src/shared/LearnFlow.Shared.BuildingBlocks/"]

RUN dotnet restore "src/services/courses/LearnFlow.Courses.API/LearnFlow.Courses.API.csproj"

COPY . .

RUN dotnet publish "src/services/courses/LearnFlow.Courses.API/LearnFlow.Courses.API.csproj" \
    -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "LearnFlow.Courses.API.dll"]