FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ApexDataExtracter.csproj", "./"]
RUN dotnet restore "ApexDataExtracter.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "ApexDataExtracter.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ApexDataExtracter.csproj" -c Release -o /app/publish


FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY appsettings.json /app/secrets.json

ENTRYPOINT ["dotnet", "ApexDataExtracter.dll"]
