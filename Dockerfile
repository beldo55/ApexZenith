# ---- Stage 1: build ----
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy the project file first so the restore layer is cached unless deps change
COPY ApexZenith/ApexZenith.csproj ./ApexZenith/
RUN dotnet restore ApexZenith/ApexZenith.csproj

# Copy the rest of the source and publish
COPY . .
RUN dotnet publish ApexZenith/ApexZenith.csproj -c Release -o /app/publish /p:UseAppHost=false

# ---- Stage 2: runtime ----
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Render routes to the port the container exposes; bind Kestrel to it.
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080

ENTRYPOINT ["dotnet", "ApexZenith.dll"]
