namespace TrendyolFinance.Domain.Common;

/// <summary>Tüm kalıcı varlıklar için temel sınıf.</summary>
public abstract class Entity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? UpdatedAt { get; set; }
}

/// <summary>Bir tenant'a (kiracıya) ait olan varlıkları işaretler; global sorgu filtresi bunu kullanır.</summary>
public interface ITenantScoped
{
    Guid TenantId { get; set; }
}
