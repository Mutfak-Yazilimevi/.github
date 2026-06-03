using LeatherErp.Domain.Enums;

namespace LeatherErp.Domain.Common;

/// <summary>
/// Deri alan birimleri arasında dönüşüm yapan tek otorite.
/// Sistem genelinde miktarlar kanonik olarak <see cref="UnitOfMeasure.Dm2"/> (desimetrekare) saklanır;
/// kullanıcıya gösterirken seçili birime çevrilir.
/// </summary>
public static class UnitConverter
{
    /// <summary>1 ayak kare (sqft) = 9.2903 dm² (uluslararası kabul, 1 ft = 30.48 cm).</summary>
    public const decimal SqftToDm2 = 9.2903m;

    /// <summary>Verilen miktarı, kaynak birimden kanonik dm²'ye çevirir.</summary>
    public static decimal ToDm2(decimal value, UnitOfMeasure from) => from switch
    {
        UnitOfMeasure.Dm2 => value,
        UnitOfMeasure.SquareFoot => value * SqftToDm2,
        _ => throw new ArgumentOutOfRangeException(nameof(from), from, "Bilinmeyen ölçü birimi.")
    };

    /// <summary>Kanonik dm² miktarını hedef birime çevirir.</summary>
    public static decimal FromDm2(decimal valueInDm2, UnitOfMeasure to) => to switch
    {
        UnitOfMeasure.Dm2 => valueInDm2,
        UnitOfMeasure.SquareFoot => valueInDm2 / SqftToDm2,
        _ => throw new ArgumentOutOfRangeException(nameof(to), to, "Bilinmeyen ölçü birimi.")
    };

    /// <summary>Bir birimden diğerine doğrudan dönüşüm.</summary>
    public static decimal Convert(decimal value, UnitOfMeasure from, UnitOfMeasure to)
        => FromDm2(ToDm2(value, from), to);
}
