# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

WORKDIR /src

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS copy-files
COPY src/Domain/ ./Domain
COPY src/Application/ ./Application
COPY src/Infrastructure/ ./Infrastructure
COPY src/Presentation/ ./Presentation
COPY src/Server/ ./Server
COPY Directory.Build.props .

# Restore nuget packages
FROM copy-files AS restore
RUN dotnet restore "./Server/Server.csproj"

# build Domain project
FROM restore AS build-domain
ARG BUILD_CONFIGURATION=Release
RUN dotnet build "./Domain/Domain.csproj" -c $BUILD_CONFIGURATION -o /app/build

# build Application project
FROM build-domain AS build-application
ARG BUILD_CONFIGURATION
RUN dotnet build "./Application/Application.csproj" -c $BUILD_CONFIGURATION -o /app/build

# build Infrastructure project
FROM build-application AS build-infrastructure
ARG BUILD_CONFIGURATION
RUN dotnet build "./Infrastructure/Infrastructure.csproj" -c $BUILD_CONFIGURATION -o /app/build

# build Presentation project
FROM build-infrastructure AS build-presentation
ARG BUILD_CONFIGURATION
RUN dotnet build "./Presentation/Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/build

# build Server project
FROM build-presentation AS build-server
ARG BUILD_CONFIGURATION #files already copied in restore stage
RUN dotnet build "./Server/Server.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build-server AS publish
ARG BUILD_CONFIGURATION
RUN dotnet publish "./Server/Server.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT dotnet Server.dll