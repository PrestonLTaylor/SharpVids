FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 5000
EXPOSE 5001

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["SharpVids/SharpVids/SharpVids.csproj", "SharpVids/SharpVids/"]
COPY ["SharpVids/SharpVids.Client/SharpVids.Client.csproj", "SharpVids/SharpVids.Client/"]
RUN dotnet restore "./SharpVids/SharpVids/SharpVids.csproj"
COPY . .
WORKDIR "/src/SharpVids/SharpVids"
RUN dotnet build "./SharpVids.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "./SharpVids.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SharpVids.dll"]