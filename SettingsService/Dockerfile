FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["SettingsService/SettingsService.csproj", "SettingsService/"]
COPY ["Common/Common.csproj", "Common/"]
RUN dotnet restore "SettingsService/SettingsService.csproj"
COPY SettingsService SettingsService 
COPY Common Common
WORKDIR "/src/SettingsService"
RUN dotnet build "SettingsService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SettingsService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SettingsService.dll"]
