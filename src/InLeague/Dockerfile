FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src
COPY ["InLeague.csproj", "./"]
RUN dotnet restore "InLeague.csproj"
COPY . .
RUN dotnet build "InLeague.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "InLeague.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "InLeague.dll"]
