using LeatherErp.Domain.Enums;

namespace LeatherErp.Api.Models;

public record LoginRequest(string Username, string Password);
public record LoginResponse(string Token, DateTime ExpiresAt, string Username, string Role);

public record SupplierRequest(string Name, string? Phone, string? Email, string? Notes);

public record LeatherTypeRequest(string Name, string? Kind, string? Color, decimal? ThicknessMm, decimal LowStockThresholdDm2);

/// <summary>Yeni deri lotu girişi. Miktar, gönderilen <see cref="Unit"/> biriminde alınır ve dm²'ye çevrilir.</summary>
public record LeatherLotRequest(
    Guid LeatherTypeId,
    Guid? SupplierId,
    string? LotNumber,
    DateTime? PurchaseDate,
    decimal Quantity,
    UnitOfMeasure Unit,
    decimal UnitCostPerDm2);

public record ProductRequest(string Name, string? Sku, string? Category, bool IsActive);

public record RecipeRequest(
    Guid LeatherTypeId,
    decimal NetLeatherDm2,
    decimal WasteRate,
    decimal LaborCost,
    decimal OverheadCost);

public record ProductionOrderRequest(Guid ProductId, int Quantity, string? Notes);

public record SalesOrderRequest(
    Guid ProductId,
    int Quantity,
    decimal UnitPrice,
    string? CustomerName,
    string? Notes,
    DateTime? SaleDate);

public record SettingsRequest(
    UnitOfMeasure DisplayUnit,
    string CurrencyCode,
    decimal VatRate,
    decimal DefaultWasteRate,
    decimal DefaultProfitMargin);

/// <summary>Reçete/serbest girdiyle birim maliyet hesabı isteği.</summary>
public record CostCalcRequest(
    decimal NetLeatherDm2,
    decimal WasteRate,
    decimal UnitCostPerDm2,
    decimal LaborCost,
    decimal OverheadCost);

public record PriceCalcRequest(decimal UnitCost, decimal ProfitMargin, decimal VatRate);

public record ReversePriceRequest(decimal UnitCost, decimal TargetSalePrice, decimal VatRate);
