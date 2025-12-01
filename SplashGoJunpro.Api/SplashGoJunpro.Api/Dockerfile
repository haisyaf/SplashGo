# Base runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy ONLY the csproj first
COPY ["SplashGoJunpro.Api/SplashGoJunpro.Api/SplashGoJunpro.Api.csproj", "SplashGoJunpro.Api/SplashGoJunpro.Api/"]

# Restore dependencies
RUN dotnet restore "SplashGoJunpro.Api/SplashGoJunpro.Api/SplashGoJunpro.Api.csproj"

# Copy ALL project files
COPY . .

# Move into the project directory
WORKDIR "/src/SplashGoJunpro.Api/SplashGoJunpro.Api"

# Build the project
RUN dotnet build "SplashGoJunpro.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "SplashGoJunpro.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Railway uses PORT → ASP.NET must listen on it
ENV ASPNETCORE_URLS=http://0.0.0.0:8080

ENTRYPOINT ["dotnet", "SplashGoJunpro.Api.dll"]
