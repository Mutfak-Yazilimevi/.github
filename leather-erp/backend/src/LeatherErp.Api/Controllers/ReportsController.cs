using LeatherErp.Application.Dtos;
using LeatherErp.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LeatherErp.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly ReportService _reports;
    public ReportsController(ReportService reports) => _reports = reports;

    /// <summary>Panel için genel özet KPI'ları (stok, mamul, üretim).</summary>
    [HttpGet("summary")]
    public async Task<ActionResult<ReportSummary>> Summary() => Ok(await _reports.GetSummaryAsync());

    /// <summary>Ürün bazında kârlılık tablosu.</summary>
    [HttpGet("profitability")]
    public async Task<ActionResult<List<ProductProfitability>>> Profitability()
        => Ok(await _reports.GetProfitabilityAsync());
}
