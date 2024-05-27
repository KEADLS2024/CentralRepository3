# Use ASP.NET Core runtime as base image
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Use .NET SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy the project file and restore dependencies
COPY ["PaymentService3/PaymentService3.csproj", "PaymentService3/"]
RUN dotnet restore "PaymentService3/PaymentService3.csproj"

# Copy the remaining source code and build the application
COPY . .
WORKDIR "/src/PaymentService3"
RUN dotnet build "PaymentService3.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish the application
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "PaymentService3.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Create the final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PaymentService3.dll"]
