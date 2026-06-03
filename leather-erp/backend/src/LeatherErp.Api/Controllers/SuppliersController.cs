using LeatherErp.Api.Models;
using LeatherErp.Application.Common;
using LeatherErp.Domain.Entities;
using LeatherErp.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LeatherErp.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class SuppliersController : ControllerBase
{
    private readonly AppDbContext _db;
    public SuppliersController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<List<Supplier>>> GetAll()
        => await _db.Suppliers.AsNoTracking().OrderBy(s => s.Name).ToListAsync();

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Supplier>> Get(Guid id)
        => await _db.Suppliers.FindAsync(id) is { } s ? s : throw new NotFoundException("Tedarikçi bulunamadı.");

    [HttpPost]
    public async Task<ActionResult<Supplier>> Create(SupplierRequest req)
    {
        var supplier = new Supplier { Name = req.Name, Phone = req.Phone, Email = req.Email, Notes = req.Notes };
        _db.Suppliers.Add(supplier);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = supplier.Id }, supplier);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<Supplier>> Update(Guid id, SupplierRequest req)
    {
        var supplier = await _db.Suppliers.FindAsync(id) ?? throw new NotFoundException("Tedarikçi bulunamadı.");
        supplier.Name = req.Name;
        supplier.Phone = req.Phone;
        supplier.Email = req.Email;
        supplier.Notes = req.Notes;
        supplier.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return Ok(supplier);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var supplier = await _db.Suppliers.FindAsync(id) ?? throw new NotFoundException("Tedarikçi bulunamadı.");
        _db.Suppliers.Remove(supplier);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
