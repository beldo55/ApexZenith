# ---- Stage 1: build ----
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files first so restore is cached unless deps change
COPY *.sln .
COPY src/IbtechApplication.Web/*.csproj            ./src/IbtechApplication.Web/
COPY src/IbtechApplication.Application/*.csproj     ./src/IbtechApplication.Application/
COPY src/IbtechApplication.Domain/*.csproj          ./src/IbtechApplication.Domain/
COPY src/IbtechApplication.Infrastructure/*.csproj  ./src/IbtechApplication.Infrastructure/
RUN dotnet restore

# Copy the rest and publish the web project
COPY . .
RUN dotnet publish src/IbtechApplication.Web/IbtechApplication.Web.csproj \
    -c Release -o /app/publish --no-restore

# ---- Stage 2: runtime ----
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "IbtechApplication.Web.dll"]