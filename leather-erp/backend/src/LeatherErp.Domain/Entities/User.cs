using LeatherErp.Domain.Common;
using LeatherErp.Domain.Enums;

namespace LeatherErp.Domain.Entities;

/// <summary>Uygulama kullanıcısı. Parolalar PBKDF2 (salt'lı) hash olarak saklanır.</summary>
public class User : BaseEntity
{
    public string Username { get; set; } = string.Empty;
    public string? FullName { get; set; }
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Operator;
    public bool IsActive { get; set; } = true;
}
