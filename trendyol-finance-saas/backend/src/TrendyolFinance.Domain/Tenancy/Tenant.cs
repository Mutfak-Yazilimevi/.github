using TrendyolFinance.Domain.Common;
using TrendyolFinance.Domain.Sellers;

namespace TrendyolFinance.Domain.Tenancy;

/// <summary>Bir abone işletme (SaaS kiracısı). Birden çok Trendyol mağazası bağlanabilir.</summary>
public class Tenant : Entity
{
    public required string Name { get; set; }
    public string? TaxNumber { get; set; }
    public SubscriptionPlan Plan { get; set; } = SubscriptionPlan.Trial;
    public bool IsActive { get; set; } = true;

    public ICollection<AppUser> Users { get; set; } = new List<AppUser>();
    public ICollection<SellerAccount> SellerAccounts { get; set; } = new List<SellerAccount>();
}

public enum SubscriptionPlan
{
    Trial = 0,
    Basic = 1,
    Pro = 2,
    Enterprise = 3
}

/// <summary>Tenant kapsamında uygulama kullanıcısı.</summary>
public class AppUser : Entity, ITenantScoped
{
    public Guid TenantId { get; set; }
    public required string Email { get; set; }
    public required string DisplayName { get; set; }
    public UserRole Role { get; set; } = UserRole.Owner;
}

public enum UserRole
{
    Owner = 0,
    Accountant = 1,
    Viewer = 2
}
