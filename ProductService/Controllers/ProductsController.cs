using FluentValidation;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductService.Data;
using ProductService.DataTransferObjects;
using ProductService.Models;

namespace ProductService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ProductDbContext _context;
    private readonly IValidator<ProductRequest> _validator;

    public ProductsController(
        ProductDbContext context,
        IValidator<ProductRequest> createValidator)
    {
        _context = context;
        _validator = createValidator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var products = await _context.Products.ToListAsync();
        return Ok(products.Adapt<List<ProductResponse>>());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product is null) return NotFound();
        return Ok(product.Adapt<ProductResponse>());
    }

    [HttpGet("filter")]
    public async Task<IActionResult> Filter(
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        [FromQuery] bool? inStock)
    {
        var query = _context.Products.AsQueryable();

        if (minPrice.HasValue)
            query = query.Where(p => p.Price >= minPrice.Value);

        if (maxPrice.HasValue)
            query = query.Where(p => p.Price <= maxPrice.Value);

        if (inStock.HasValue && inStock.Value)
            query = query.Where(p => p.Stock > 0);

        var products = await query.ToListAsync();
        return Ok(products.Adapt<List<ProductResponse>>());
    }

    [HttpPost]
    public async Task<IActionResult> Create(ProductRequest request)
    {
        var result = await _validator.ValidateAsync(request);
        if (!result.IsValid)
            return BadRequest(result.Errors.Select(e => e.ErrorMessage));

        var product = request.Adapt<Product>();
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product.Adapt<ProductResponse>());
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, ProductRequest request)
    {
        var result = await _validator.ValidateAsync(request);
        if (!result.IsValid)
            return BadRequest(result.Errors.Select(e => e.ErrorMessage));

        var product = await _context.Products.FindAsync(id);
        if (product is null) return NotFound();

        request.Adapt(product);
        await _context.SaveChangesAsync();
        return Ok(product.Adapt<ProductResponse>());
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product is null) return NotFound();

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}