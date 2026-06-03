using LeatherErp.Api.Models;
using LeatherErp.Application.Common;
using LeatherErp.Application.Services;
using LeatherErp.Domain.Entities;
using LeatherErp.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LeatherErp.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class ProductionController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly ProductionService _production;

    public ProductionController(AppDbContext db, ProductionService production)
    {
        _db = db;
        _production = production;
    }

    [HttpGet]
    public async Task<ActionResult<List<ProductionOrder>>> GetAll()
        => await _db.ProductionOrders.AsNoTracking()
            .Include(o => o.Product)
            .OrderByDescending(o => o.OrderDate).ToListAsync();

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductionOrder>> Get(Guid id)
        => await _db.ProductionOrders.AsNoTracking().Include(o => o.Product)
            .FirstOrDefaultAsync(o => o.Id == id) ?? throw new NotFoundException("Üretim emri bulunamadı.");

    /// <summary>Taslak üretim emri oluşturur (stok henüz düşülmez).</summary>
    [HttpPost]
    public async Task<ActionResult<ProductionOrder>> Create(ProductionOrderRequest req)
    {
        if (req.Quantity <= 0) throw new BusinessRuleException("Üretim adedi sıfırdan büyük olmalıdır.");
        if (!await _db.Products.AnyAsync(p => p.Id == req.ProductId))
            throw new NotFoundException("Ürün bulunamadı.");

        var order = new ProductionOrder { ProductId = req.ProductId, Quantity = req.Quantity, Notes = req.Notes };
        _db.ProductionOrders.Add(order);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = order.Id }, order);
    }

    /// <summary>Üretim emrini onaylar: deri stoğundan FIFO düşer, mamul stoğa ekler.</summary>
    [HttpPost("{id:guid}/confirm")]
    public async Task<ActionResult<ProductionOrder>> Confirm(Guid id)
        => Ok(await _production.ConfirmAsync(id));

    /// <summary>Eldeki mamul stoğunu listeler.</summary>
    [HttpGet("/api/finished-goods")]
    public async Task<ActionResult<List<FinishedGoodsInventory>>> FinishedGoods()
        => await _db.FinishedGoods.AsNoTracking().Include(f => f.Product).ToListAsync();
}
