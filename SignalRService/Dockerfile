FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["SignalRService/SignalRService.csproj", "SignalRService/"]
COPY ["Common/Common.csproj", "Common/"]
RUN dotnet restore "SignalRService/SignalRService.csproj"
COPY SignalRService SignalRService 
COPY Common Common
WORKDIR "/src/SignalRService"
RUN dotnet build "SignalRService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SignalRService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SignalRService.dll"]
