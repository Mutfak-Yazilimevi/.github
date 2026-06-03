# --- Build aşaması ---
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY *.sln ./
COPY src/LeatherErp.Domain/*.csproj src/LeatherErp.Domain/
COPY src/LeatherErp.Application/*.csproj src/LeatherErp.Application/
COPY src/LeatherErp.Infrastructure/*.csproj src/LeatherErp.Infrastructure/
COPY src/LeatherErp.Api/*.csproj src/LeatherErp.Api/
COPY tests/LeatherErp.Application.Tests/*.csproj tests/LeatherErp.Application.Tests/
RUN dotnet restore src/LeatherErp.Api/LeatherErp.Api.csproj
COPY . .
RUN dotnet publish src/LeatherErp.Api/LeatherErp.Api.csproj -c Release -o /app/publish

# --- Çalışma aşaması ---
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "LeatherErp.Api.dll"]
