# Use the .NET SDK 7.0 image to build the application
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copy the project file and restore dependencies
COPY ["DLS_Catalog_Service_3.0.csproj", "./"]
RUN dotnet restore "DLS_Catalog_Service_3.0.csproj"

# Copy the remaining files and build the application
COPY . .
RUN dotnet build "DLS_Catalog_Service_3.0.csproj" -c Release -o /app/build

# Use the ASP.NET Core runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app

# Copy the build output to the runtime image
COPY --from=build /app/build .

# Expose ports
EXPOSE 80
EXPOSE 443

# Set the entry point
ENTRYPOINT ["dotnet", "DLS_Catalog_Service_3.0.dll"]
