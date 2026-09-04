# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy each layer's .csproj separately (better restore layer caching)
COPY ["CulturalCMS.API/CulturalCMS.API.csproj", "CulturalCMS.API/"]
COPY ["CulturalCMS.Application/CulturalCMS.Application.csproj", "CulturalCMS.Application/"]
COPY ["CulturalCMS.Domain/CulturalCMS.Domain.csproj", "CulturalCMS.Domain/"]
COPY ["CulturalCMS.Infrastructure/CulturalCMS.Infrastructure.csproj", "CulturalCMS.Infrastructure/"]

# Restore from the API project, which pulls in the referenced layers
RUN dotnet restore "CulturalCMS.API/CulturalCMS.API.csproj"

# Copy the rest of the source
COPY . .

# Build and publish the API in Release mode
WORKDIR "/src/CulturalCMS.API"
RUN dotnet publish "CulturalCMS.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Copy the published output from the build stage
COPY --from=build /app/publish .

# Application entry point
ENTRYPOINT ["dotnet", "CulturalCMS.API.dll"]