# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

ENV DOTNET_EnableDiagnostics=0

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["SportComplexApp.Web/SportComplexApp.Web.csproj", "SportComplexApp.Web/"]
COPY ["SportComplexApp.Common/SportComplexApp.Common.csproj", "SportComplexApp.Common/"]
COPY ["SportComplexApp.Data/SportComplexApp.Data.csproj", "SportComplexApp.Data/"]
COPY ["SportComplexApp.Data.Models/SportComplexApp.Data.Models.csproj", "SportComplexApp.Data.Models/"]
COPY ["SportComplexApp.Services.Data/SportComplexApp.Services.Data.csproj", "SportComplexApp.Services.Data/"]
COPY ["SportComplexApp.Web.ViewModels/SportComplexApp.Web.ViewModels.csproj", "SportComplexApp.Web.ViewModels/"]
COPY ["SportComplexApp.Services.Mapping/SportComplexApp.Services.Mapping.csproj", "SportComplexApp.Services.Mapping/"]
RUN dotnet restore "./SportComplexApp.Web/SportComplexApp.Web.csproj"
COPY . .
WORKDIR "/src/SportComplexApp.Web"
RUN dotnet build "./SportComplexApp.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./SportComplexApp.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SportComplexApp.Web.dll"]