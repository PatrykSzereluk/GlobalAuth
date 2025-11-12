# ===== build =====
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["GlobalAuth.Api/GlobalAuth.Api.csproj", "GlobalAuth.Api/"]
COPY ["GlobalAuth.Application/GlobalAuth.Application.csproj", "GlobalAuth.Application/"]
COPY ["GlobalAuth.Domain/GlobalAuth.Domain.csproj", "GlobalAuth.Domain/"]
COPY ["GlobalAuth.Infrastructure/GlobalAuth.Infrastructure.csproj", "GlobalAuth.Infrastructure/"]
RUN dotnet restore "GlobalAuth.Api/GlobalAuth.Api.csproj"

COPY . .
WORKDIR /src/GlobalAuth.Api
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# ===== runtime =====
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "GlobalAuth.Api.dll"]