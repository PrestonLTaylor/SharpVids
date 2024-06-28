FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 5000
EXPOSE 5001

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["SharpVids/src/SharpVids/SharpVids.csproj", "SharpVids/src/SharpVids/"]
COPY ["SharpVids/src/SharpVids.Client/SharpVids.Client.csproj", "SharpVids/src/SharpVids.Client/"]
RUN dotnet restore "./SharpVids/src/SharpVids/SharpVids.csproj"
COPY . .
WORKDIR "SharpVids/src/SharpVids"
RUN dotnet build "./SharpVids.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "./SharpVids.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SharpVids.dll"]