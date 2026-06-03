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
[Route("api/leather-types")]
public class LeatherTypesController : ControllerBase
{
    private readonly AppDbContext _db;
    public LeatherTypesController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<List<LeatherType>>> GetAll()
        => await _db.LeatherTypes.AsNoTracking().OrderBy(t => t.Name).ToListAsync();

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<LeatherType>> Get(Guid id)
        => await _db.LeatherTypes.FindAsync(id) is { } t ? t : throw new NotFoundException("Deri tipi bulunamadı.");

    [HttpPost]
    public async Task<ActionResult<LeatherType>> Create(LeatherTypeRequest req)
    {
        var type = new LeatherType
        {
            Name = req.Name, Kind = req.Kind, Color = req.Color,
            ThicknessMm = req.ThicknessMm, LowStockThresholdDm2 = req.LowStockThresholdDm2
        };
        _db.LeatherTypes.Add(type);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = type.Id }, type);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<LeatherType>> Update(Guid id, LeatherTypeRequest req)
    {
        var type = await _db.LeatherTypes.FindAsync(id) ?? throw new NotFoundException("Deri tipi bulunamadı.");
        type.Name = req.Name;
        type.Kind = req.Kind;
        type.Color = req.Color;
        type.ThicknessMm = req.ThicknessMm;
        type.LowStockThresholdDm2 = req.LowStockThresholdDm2;
        type.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return Ok(type);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var type = await _db.LeatherTypes.FindAsync(id) ?? throw new NotFoundException("Deri tipi bulunamadı.");
        _db.LeatherTypes.Remove(type);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
