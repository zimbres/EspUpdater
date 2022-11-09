FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/EspUpdater/EspUpdater.csproj", "src/EspUpdater/"]
RUN dotnet restore "src/EspUpdater/EspUpdater.csproj"
COPY . .
WORKDIR "/src/src/EspUpdater"
RUN dotnet build "EspUpdater.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EspUpdater.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EspUpdater.dll"]