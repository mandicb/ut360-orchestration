#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Nuget.config", "."]
COPY ["Services/utrust360_hsm_orchestration_service/Orchestration.API/Orchestration.API.csproj", "Services/utrust360_hsm_orchestration_service/Orchestration.API/"]
RUN dotnet restore "Services/utrust360_hsm_orchestration_service/Orchestration.API/Orchestration.API.csproj"
COPY . .
WORKDIR "/src/Services/utrust360_hsm_orchestration_service/Orchestration.API"
RUN dotnet build "Orchestration.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Orchestration.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Orchestration.API.dll"]