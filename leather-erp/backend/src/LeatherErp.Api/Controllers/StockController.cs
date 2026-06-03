using LeatherErp.Api.Models;
using LeatherErp.Application.Common;
using LeatherErp.Application.Dtos;
using LeatherErp.Application.Services;
using LeatherErp.Domain.Common;
using LeatherErp.Domain.Entities;
using LeatherErp.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LeatherErp.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/stock")]
public class StockController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly StockService _stock;

    public StockController(AppDbContext db, StockService stock)
    {
        _db = db;
        _stock = stock;
    }

    /// <summary>Deri tipi bazında toplam kalan stok ve değer.</summary>
    [HttpGet("levels")]
    public async Task<ActionResult<List<StockLevelDto>>> Levels() => await _stock.GetStockLevelsAsync();

    /// <summary>Eşiğin altına düşmüş deri tipleri.</summary>
    [HttpGet("low")]
    public async Task<ActionResult<List<StockLevelDto>>> Low() => await _stock.GetLowStockAsync();

    /// <summary>Tüm deri lotlarını listeler (opsiyonel deri tipi filtresi).</summary>
    [HttpGet("lots")]
    public async Task<ActionResult<List<LeatherLot>>> Lots([FromQuery] Guid? leatherTypeId)
    {
        var query = _db.LeatherLots.AsNoTracking().Include(l => l.LeatherType).Include(l => l.Supplier).AsQueryable();
        if (leatherTypeId is { } id) query = query.Where(l => l.LeatherTypeId == id);
        return await query.OrderByDescending(l => l.PurchaseDate).ToListAsync();
    }

    /// <summary>Yeni deri lotu girişi. Miktar gönderilen birimde alınır, dm²'ye çevrilerek saklanır.</summary>
    [HttpPost("lots")]
    public async Task<ActionResult<LeatherLot>> AddLot(LeatherLotRequest req)
    {
        if (req.Quantity <= 0) throw new BusinessRuleException("Miktar sıfırdan büyük olmalıdır.");
        if (!await _db.LeatherTypes.AnyAsync(t => t.Id == req.LeatherTypeId))
            throw new NotFoundException("Deri tipi bulunamadı.");

        var quantityDm2 = UnitConverter.ToDm2(req.Quantity, req.Unit);
        var lot = new LeatherLot
        {
            LeatherTypeId = req.LeatherTypeId,
            SupplierId = req.SupplierId,
            LotNumber = req.LotNumber,
            PurchaseDate = req.PurchaseDate ?? DateTime.UtcNow,
            QuantityDm2 = quantityDm2,
            RemainingDm2 = quantityDm2,
            UnitCostPerDm2 = req.UnitCostPerDm2
        };
        _db.LeatherLots.Add(lot);
        _db.StockMovements.Add(new StockMovement
        {
            LeatherLot = lot,
            Direction = Domain.Enums.MovementDirection.In,
            QuantityDm2 = quantityDm2,
            Reason = "Stok girişi (alım)"
        });
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Lots), new { leatherTypeId = lot.LeatherTypeId }, lot);
    }

    /// <summary>Bir lotun stok hareketlerini döndürür.</summary>
    [HttpGet("lots/{lotId:guid}/movements")]
    public async Task<ActionResult<List<StockMovement>>> Movements(Guid lotId)
        => await _db.StockMovements.AsNoTracking()
            .Where(m => m.LeatherLotId == lotId)
            .OrderByDescending(m => m.MovementDate).ToListAsync();
}
