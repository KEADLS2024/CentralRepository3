# AggregatorService Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["AggregatorService.csproj", "./"]
RUN dotnet restore "AggregatorService.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "AggregatorService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AggregatorService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AggregatorService.dll"]
