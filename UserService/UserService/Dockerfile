# Brug det officielle ASP.NET Core runtime billede som base
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Brug det officielle .NET SDK billede til at bygge appen
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["UserService.csproj", "./"]
RUN dotnet restore "UserService.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "UserService.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publicer build-output
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "UserService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Brug runtime billedet til at køre appen
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UserService.dll"]
