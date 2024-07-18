FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["TTLockDotnetASP/TTLockDotnetASP.csproj", "TTLockDotnetASP/"]
RUN dotnet restore "TTLockDotnetASP/TTLockDotnetASP.csproj"
COPY . .
WORKDIR "/src/TTLockDotnetASP"
RUN dotnet build "TTLockDotnetASP.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "TTLockDotnetASP.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TTLockDotnetASP.dll"]
