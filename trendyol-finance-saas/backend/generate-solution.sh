#!/usr/bin/env bash
# .sln dosyasını ve proje referanslarını güvenilir şekilde oluşturur.
# Bu ortamda dotnet SDK bulunmadığından .sln elle yazılmadı; .NET 8 SDK kurulu
# bir makinede bu scripti çalıştırın.
set -euo pipefail
cd "$(dirname "$0")"

dotnet new sln -n TrendyolFinance --force

dotnet sln add \
  src/TrendyolFinance.Domain/TrendyolFinance.Domain.csproj \
  src/TrendyolFinance.Application/TrendyolFinance.Application.csproj \
  src/TrendyolFinance.Integration/TrendyolFinance.Integration.csproj \
  src/TrendyolFinance.Infrastructure/TrendyolFinance.Infrastructure.csproj \
  src/TrendyolFinance.Api/TrendyolFinance.Api.csproj \
  tests/TrendyolFinance.UnitTests/TrendyolFinance.UnitTests.csproj

echo "Çözüm oluşturuldu. Derlemek için: dotnet build"
echo "Testler için:        dotnet test"
