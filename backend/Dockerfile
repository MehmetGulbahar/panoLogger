FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY PanoLogger.Backend.slnx ./
COPY src/PanoLogger.Api/PanoLogger.Api.csproj src/PanoLogger.Api/
COPY src/PanoLogger.Application/PanoLogger.Application.csproj src/PanoLogger.Application/
COPY src/PanoLogger.Domain/PanoLogger.Domain.csproj src/PanoLogger.Domain/
COPY src/PanoLogger.Infrastructure/PanoLogger.Infrastructure.csproj src/PanoLogger.Infrastructure/

RUN dotnet restore PanoLogger.Backend.slnx

COPY . .
RUN dotnet publish src/PanoLogger.Api/PanoLogger.Api.csproj \
    --configuration Release \
    --no-restore \
    --output /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://0.0.0.0:8080

EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "PanoLogger.Api.dll"]
