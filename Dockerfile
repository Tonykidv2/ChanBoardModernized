####Regular Deployment

## See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.
#
## This stage is used when running from VS in fast mode (Default for Debug configuration)
#FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
#USER $APP_UID
#WORKDIR /app
#EXPOSE 8080
#EXPOSE 8081
#
## This stage is used to build the service project
#FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
#ARG BUILD_CONFIGURATION=Release
#WORKDIR /src
#COPY ["ChanBoardModernized.API/ChanBoardModernized.API.csproj", "ChanBoardModernized.API/"]
#RUN dotnet restore "./ChanBoardModernized.API/ChanBoardModernized.API.csproj"
#COPY . .
#WORKDIR "/src/ChanBoardModernized.API"
#RUN dotnet build "./ChanBoardModernized.API.csproj" -c $BUILD_CONFIGURATION -o /app/build
#
## This stage is used to publish the service project to be copied to the final stage
#FROM build AS publish
#ARG BUILD_CONFIGURATION=Release
#RUN dotnet publish "./ChanBoardModernized.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false
#
## This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
#FROM base AS final
#WORKDIR /app
#COPY --from=publish /app/publish .
#ENTRYPOINT ["dotnet", "ChanBoardModernized.API.dll"]

###################################

#Raspberry Pi (Pirate Box) Deployment
# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
#ENV ASPNETCORE_URLS=http://+:8080

# Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution file
COPY ["ChanBoardModernized.sln", "./"]

# Copy project files FIRST (for layer caching)
COPY ["ChanBoardModernized.API/ChanBoardModernized.API.csproj", "ChanBoardModernized.API/"]
COPY ["ChanBoardModernized.Shared.Components/ChanBoardModernized.Shared.Components.csproj", "ChanBoardModernized.Shared.Components/"]

# Restore
RUN dotnet restore "./ChanBoardModernized.API/ChanBoardModernized.API.csproj"

# Copy everything else
COPY . .

# Publish
WORKDIR "/src/ChanBoardModernized.API"
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Final image
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ChanBoardModernized.API.dll"]
