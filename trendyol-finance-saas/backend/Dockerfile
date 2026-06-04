# --- build ---
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Proje dosyalarını kopyala + restore (katman önbelleği için ayrı adım).
COPY . .
RUN dotnet restore src/TrendyolFinance.Api/TrendyolFinance.Api.csproj
RUN dotnet publish src/TrendyolFinance.Api/TrendyolFinance.Api.csproj -c Release -o /app/publish --no-restore

# --- runtime ---
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "TrendyolFinance.Api.dll"]
