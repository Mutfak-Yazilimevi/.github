namespace LeatherErp.Application.Common;

/// <summary>İstenen kayıt bulunamadığında fırlatılır (HTTP 404'e eşlenir).</summary>
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
}

/// <summary>İş kuralı ihlali / geçersiz işlem (HTTP 400'e eşlenir).</summary>
public class BusinessRuleException : Exception
{
    public BusinessRuleException(string message) : base(message) { }
}

/// <summary>Üretim için yeterli deri stoğu olmadığında fırlatılır.</summary>
public class InsufficientStockException : BusinessRuleException
{
    public InsufficientStockException(string message) : base(message) { }
}
