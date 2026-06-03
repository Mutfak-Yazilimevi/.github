namespace LeatherErp.Domain.Enums;

/// <summary>Ölçü birimi. Sistemde deri miktarları kanonik olarak dm² (desimetrekare) saklanır.</summary>
public enum UnitOfMeasure
{
    /// <summary>Desimetrekare (dm²) — kanonik depolama birimi.</summary>
    Dm2 = 0,

    /// <summary>Ayak kare (square foot / sqft).</summary>
    SquareFoot = 1
}

/// <summary>Stok hareket yönü.</summary>
public enum MovementDirection
{
    /// <summary>Stok girişi (alım).</summary>
    In = 0,

    /// <summary>Stok çıkışı (üretimde tüketim).</summary>
    Out = 1,

    /// <summary>Fire / kesim kaybı.</summary>
    Waste = 2,

    /// <summary>Manuel düzeltme.</summary>
    Adjustment = 3
}

/// <summary>Üretim emri durumu.</summary>
public enum ProductionStatus
{
    /// <summary>Taslak — henüz stok düşülmedi.</summary>
    Draft = 0,

    /// <summary>Onaylandı — stok düşüldü, mamul stoğa eklendi.</summary>
    Confirmed = 1,

    /// <summary>İptal edildi.</summary>
    Cancelled = 2
}

/// <summary>Kullanıcı rolü.</summary>
public enum UserRole
{
    /// <summary>Tüm yetkilere sahip yönetici.</summary>
    Admin = 0,

    /// <summary>Üretim ve stok işlemleri yapan operatör.</summary>
    Operator = 1
}
