using LeatherErp.Api.Models;
using LeatherErp.Domain.Entities;
using LeatherErp.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LeatherErp.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class SettingsController : ControllerBase
{
    private readonly AppDbContext _db;
    public SettingsController(AppDbContext db) => _db = db;

    /// <summary>Uygulama ayarlarını döndürür (yoksa varsayılan oluşturur).</summary>
    [HttpGet]
    public async Task<ActionResult<AppSettings>> Get()
    {
        var settings = await _db.Settings.FirstOrDefaultAsync();
        if (settings is null)
        {
            settings = new AppSettings();
            _db.Settings.Add(settings);
            await _db.SaveChangesAsync();
        }
        return Ok(settings);
    }

    /// <summary>Ayarları günceller.</summary>
    [HttpPut]
    public async Task<ActionResult<AppSettings>> Update(SettingsRequest req)
    {
        var settings = await _db.Settings.FirstOrDefaultAsync();
        if (settings is null)
        {
            settings = new AppSettings();
            _db.Settings.Add(settings);
        }
        settings.DisplayUnit = req.DisplayUnit;
        settings.CurrencyCode = req.CurrencyCode;
        settings.VatRate = req.VatRate;
        settings.DefaultWasteRate = req.DefaultWasteRate;
        settings.DefaultProfitMargin = req.DefaultProfitMargin;
        settings.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return Ok(settings);
    }
}
