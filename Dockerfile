FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ComicApp.Core/ComicApp.Core.csproj ./ComicApp.Core/
COPY ComicApp.Web/ComicApp.Web.csproj ./ComicApp.Web/
COPY ComicApp.API/ComicApp.API.csproj ./ComicApp.API/
COPY ComicApp.Tests/ComicApp.Tests.csproj ./ComicApp.Tests/

RUN dotnet restore ComicApp.Web/ComicApp.Web.csproj --runtime linux-x64

COPY . .

RUN dotnet publish ComicApp.Web/ComicApp.Web.csproj \
    --configuration Release \
    --output /app/publish \
    --no-restore \
    --runtime linux-x64 \
    --self-contained false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

RUN apt-get update && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

RUN groupadd --system appgroup && \
    useradd --system --gid appgroup --no-create-home appuser

COPY --from=build /app/publish .
RUN chown -R appuser:appgroup /app
USER appuser

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "ComicApp.Web.dll"]
