# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
#$APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["SplashGoJunpro.Api/SplashGoJunpro.Api/SplashGoJunpro.Api.csproj", "SplashGoJunpro.Api/SplashGoJunpro.Api/"]
RUN dotnet restore "./SplashGoJunpro.Api/SplashGoJunpro.Api/SplashGoJunpro.Api.csproj"
COPY . .
WORKDIR "/src/SplashGoJunpro.Api/SplashGoJunpro.Api"
RUN dotnet build "./SplashGoJunpro.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./SplashGoJunpro.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
# ❗ Railway uses PORT env variable → ASP.NET must listen on that port
ENV ASPNETCORE_URLS=http://0.0.0.0:8080
ENTRYPOINT ["dotnet", "SplashGoJunpro.Api.dll"]