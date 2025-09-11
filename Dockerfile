FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

COPY ./app/Api ./app
COPY ./app/web ./app/wwwroot

WORKDIR /app
EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8000
ENTRYPOINT ["dotnet", "Api.dll"]

# FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
# WORKDIR /src

# COPY . .

# RUN dotnet restore ./Api/Api.csproj
# RUN dotnet publish ./Api/Api.csproj -c Release -o /app/publish

# FROM base AS final
# WORKDIR /app
# COPY --from=build /app/publish .
# ENTRYPOINT ["dotnet", "Api.dll"]