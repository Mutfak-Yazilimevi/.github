namespace LeatherErp.Application.Dtos;

/// <summary>Bir deri tipi için toplam stok durumu (kanonik dm²).</summary>
public record StockLevelDto
{
    public Guid LeatherTypeId { get; init; }
    public string LeatherTypeName { get; init; } = string.Empty;

    /// <summary>Toplam kalan miktar (dm²).</summary>
    public decimal RemainingDm2 { get; init; }

    /// <summary>Kalan stoğun maliyet değeri (Σ kalan × lot birim maliyeti).</summary>
    public decimal StockValue { get; init; }

    /// <summary>Düşük stok eşiği (dm²).</summary>
    public decimal LowStockThresholdDm2 { get; init; }

    /// <summary>Kalan, eşiğin altındaysa true.</summary>
    public bool IsLow => RemainingDm2 < LowStockThresholdDm2;
}
