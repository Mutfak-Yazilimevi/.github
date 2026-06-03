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
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _db;
    public ProductsController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<List<Product>>> GetAll()
        => await _db.Products.AsNoTracking()
            .Include(p => p.Recipe).Include(p => p.Inventory)
            .OrderBy(p => p.Name).ToListAsync();

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Product>> Get(Guid id)
        => await _db.Products.AsNoTracking()
            .Include(p => p.Recipe).Include(p => p.Inventory)
            .FirstOrDefaultAsync(p => p.Id == id) ?? throw new NotFoundException("Ürün bulunamadı.");

    [HttpPost]
    public async Task<ActionResult<Product>> Create(ProductRequest req)
    {
        var product = new Product { Name = req.Name, Sku = req.Sku, Category = req.Category, IsActive = req.IsActive };
        _db.Products.Add(product);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = product.Id }, product);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<Product>> Update(Guid id, ProductRequest req)
    {
        var product = await _db.Products.FindAsync(id) ?? throw new NotFoundException("Ürün bulunamadı.");
        product.Name = req.Name;
        product.Sku = req.Sku;
        product.Category = req.Category;
        product.IsActive = req.IsActive;
        product.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return Ok(product);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var product = await _db.Products.FindAsync(id) ?? throw new NotFoundException("Ürün bulunamadı.");
        _db.Products.Remove(product);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>Ürün reçetesini oluşturur veya günceller (upsert).</summary>
    [HttpPut("{id:guid}/recipe")]
    public async Task<ActionResult<ProductRecipe>> SetRecipe(Guid id, RecipeRequest req)
    {
        if (!await _db.Products.AnyAsync(p => p.Id == id))
            throw new NotFoundException("Ürün bulunamadı.");
        if (!await _db.LeatherTypes.AnyAsync(t => t.Id == req.LeatherTypeId))
            throw new NotFoundException("Deri tipi bulunamadı.");
        if (req.WasteRate < 0 || req.WasteRate >= 1)
            throw new BusinessRuleException("Fire oranı [0,1) aralığında olmalıdır.");

        var recipe = await _db.Recipes.FirstOrDefaultAsync(r => r.ProductId == id);
        if (recipe is null)
        {
            recipe = new ProductRecipe { ProductId = id };
            _db.Recipes.Add(recipe);
        }
        recipe.LeatherTypeId = req.LeatherTypeId;
        recipe.NetLeatherDm2 = req.NetLeatherDm2;
        recipe.WasteRate = req.WasteRate;
        recipe.LaborCost = req.LaborCost;
        recipe.OverheadCost = req.OverheadCost;
        recipe.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return Ok(recipe);
    }
}
