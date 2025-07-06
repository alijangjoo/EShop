# Use the official ASP.NET Core runtime as the base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Use the SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["src/ApiGateways/ApiGateway/ApiGateway.csproj", "src/ApiGateways/ApiGateway/"]
RUN dotnet restore "src/ApiGateways/ApiGateway/ApiGateway.csproj"

# Copy all source code
COPY . .
WORKDIR "/src/src/ApiGateways/ApiGateway"

# Build the application
RUN dotnet build "ApiGateway.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "ApiGateway.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Create the final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Create a non-root user for security
RUN adduser --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_URLS=http://+:80

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
  CMD curl -f http://localhost:80/health || exit 1

ENTRYPOINT ["dotnet", "ApiGateway.dll"]