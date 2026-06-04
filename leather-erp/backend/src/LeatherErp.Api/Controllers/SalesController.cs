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
public class SalesController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly SalesService _sales;

    public SalesController(AppDbContext db, SalesService sales)
    {
        _db = db;
        _sales = sales;
    }

    /// <summary>Satışları listeler (en yeni önce).</summary>
    [HttpGet]
    public async Task<ActionResult<List<SalesOrder>>> GetAll()
        => await _db.SalesOrders.AsNoTracking().Include(s => s.Product)
            .OrderByDescending(s => s.SaleDate).ToListAsync();

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<SalesOrder>> Get(Guid id)
        => await _db.SalesOrders.AsNoTracking().Include(s => s.Product)
            .FirstOrDefaultAsync(s => s.Id == id) ?? throw new NotFoundException("Satış kaydı bulunamadı.");

    /// <summary>Satış oluşturur: mamul stoğundan düşer, COGS dondurulur.</summary>
    [HttpPost]
    public async Task<ActionResult<SalesOrder>> Create(SalesOrderRequest req)
    {
        var sale = await _sales.CreateAsync(req.ProductId, req.Quantity, req.UnitPrice,
            req.CustomerName, req.Notes, req.SaleDate);
        return CreatedAtAction(nameof(Get), new { id = sale.Id }, sale);
    }
}
