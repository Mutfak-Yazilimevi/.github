namespace LeatherErp.Domain.Common;

/// <summary>Tüm kalıcı varlıklar için ortak taban: kimlik ve oluşturulma/güncellenme zaman damgaları.</summary>
public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }
}
