FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/services/gateway/LearnFlow.Gateway/LearnFlow.Gateway.csproj", "src/services/gateway/LearnFlow.Gateway/"]

RUN dotnet restore "src/services/gateway/LearnFlow.Gateway/LearnFlow.Gateway.csproj"

COPY . .

RUN dotnet publish "src/services/gateway/LearnFlow.Gateway/LearnFlow.Gateway.csproj" \
    -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "LearnFlow.Gateway.dll"]