FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["FGC.Catalog.Api/FGC.Catalog.Api.csproj", "FGC.Catalog.Api/"]
COPY ["FGC.Catalog.Application/FGC.Catalog.Application.csproj", "FGC.Catalog.Application/"]
COPY ["FGC.Catalog.Infrastructure/FGC.Catalog.Infrastructure.csproj", "FGC.Catalog.Infrastructure/"]
COPY ["FGC.Catalog.Domain/FGC.Catalog.Domain.csproj", "FGC.Catalog.Domain/"]
RUN dotnet restore "FGC.Catalog.Api/FGC.Catalog.Api.csproj"

COPY . .
WORKDIR "/src/FGC.Catalog.Api"
RUN dotnet publish -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "FGC.Catalog.Api.dll"]
