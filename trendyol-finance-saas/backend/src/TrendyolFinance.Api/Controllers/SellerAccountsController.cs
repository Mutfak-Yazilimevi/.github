using Hangfire;
using Microsoft.AspNetCore.Mvc;
using TrendyolFinance.Application.Abstractions;
using TrendyolFinance.Domain.Sellers;
using TrendyolFinance.Infrastructure.Persistence;
using TrendyolFinance.Ingestion;

namespace TrendyolFinance.Api.Controllers;

public record ConnectStoreRequest(
    string StoreName, long TrendyolSellerId, string ApiKey, string ApiSecret);

/// <summary>Tenant'a Trendyol mağazası bağlama. Kimlikler şifreli saklanır, backfill arka planda başlatılır.</summary>
[ApiController]
[Route("api/[controller]")]
public class SellerAccountsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly ICredentialProtector _protector;
    private readonly ICurrentTenant _tenant;
    private readonly IBackgroundJobClient _jobs;

    public SellerAccountsController(
        AppDbContext db, ICredentialProtector protector,
        ICurrentTenant tenant, IBackgroundJobClient jobs)
    {
        _db = db;
        _protector = protector;
        _tenant = tenant;
        _jobs = jobs;
    }

    [HttpPost("connect")]
    public async Task<IActionResult> Connect([FromBody] ConnectStoreRequest req, CancellationToken ct)
    {
        if (_tenant.TenantId is null) return Unauthorized("Tenant çözülemedi.");

        var account = new SellerAccount
        {
            TenantId = _tenant.TenantId.Value,
            StoreName = req.StoreName,
            TrendyolSellerId = req.TrendyolSellerId,
            ApiKeyEncrypted = _protector.Protect(req.ApiKey),
            ApiSecretEncrypted = _protector.Protect(req.ApiSecret)
        };

        _db.SellerAccounts.Add(account);
        await _db.SaveChangesAsync(ct);

        // İlk tarihsel yükleme arka planda.
        _jobs.Enqueue<IngestionJobs>(j => j.BackfillAccountAsync(account.Id, CancellationToken.None));

        return Ok(new { account.Id, account.StoreName });
    }
}
