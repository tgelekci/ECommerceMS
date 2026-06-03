using FluentValidation;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.DataTransferObjects;
using OrderService.Models;
using System.Net.Http.Json;

namespace OrderService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly OrderDbContext _context;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IValidator<CreateOrderRequest> _createValidator;
    private readonly IValidator<UpdateOrderStatusRequest> _statusValidator;

    public OrdersController(
        OrderDbContext context,
        IHttpClientFactory httpClientFactory,
        IValidator<CreateOrderRequest> createValidator,
        IValidator<UpdateOrderStatusRequest> statusValidator)
    {
        _context = context;
        _httpClientFactory = httpClientFactory;
        _createValidator = createValidator;
        _statusValidator = statusValidator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var orders = await _context.Orders.ToListAsync();
        return Ok(orders.Adapt<List<OrderResponse>>());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order is null) return NotFound();
        return Ok(order.Adapt<OrderResponse>());
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetByUser(int userId)
    {
        var orders = await _context.Orders
            .Where(o => o.UserId == userId)
            .ToListAsync();

        if (!orders.Any())
            return NotFound($"UserId {userId} icin siparis bulunamadi.");

        return Ok(orders.Adapt<List<OrderResponse>>());
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateOrderRequest request)
    {
        var result = await _createValidator.ValidateAsync(request);
        if (!result.IsValid)
            return BadRequest(result.Errors.Select(e => e.ErrorMessage));

        var client = _httpClientFactory.CreateClient();

        var userResponse = await client.GetAsync($"http://localhost:5005/api/users/{request.UserId}");
        if (!userResponse.IsSuccessStatusCode)
            return BadRequest($"UserId {request.UserId} bulunamadi.");

        var productResponse = await client.GetAsync($"http://localhost:5001/api/products/{request.ProductId}");
        if (!productResponse.IsSuccessStatusCode)
            return BadRequest($"ProductId {request.ProductId} bulunamadi.");

        var product = await productResponse.Content.ReadFromJsonAsync<ProductDto>();
        if (product is null)
            return BadRequest("Urun bilgisi alinamadi.");

        if (request.Quantity > product.Stock)
            return BadRequest($"Yetersiz stok. Mevcut: {product.Stock}");

        var order = request.Adapt<Order>();
        order.TotalPrice = product.Price * request.Quantity;
        order.CreatedAt = DateTime.UtcNow;

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = order.Id }, order.Adapt<OrderResponse>());
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, UpdateOrderStatusRequest request)
    {
        var result = await _statusValidator.ValidateAsync(request);
        if (!result.IsValid)
            return BadRequest(result.Errors.Select(e => e.ErrorMessage));

        var order = await _context.Orders.FindAsync(id);
        if (order is null) return NotFound();

        if (order.Status == "Cancelled")
            return BadRequest("Iptal edilen siparisin durumu degistirilemez.");

        if (order.Status == "Delivered")
            return BadRequest("Teslim edilen siparisin durumu degistirilemez.");

        order.Status = request.Status;
        await _context.SaveChangesAsync();
        return Ok(order.Adapt<OrderResponse>());
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Cancel(int id)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order is null) return NotFound();

        if (order.Status == "Delivered")
            return BadRequest("Teslim edilen siparis iptal edilemez.");

        order.Status = "Cancelled";
        await _context.SaveChangesAsync();
        return Ok(order.Adapt<OrderResponse>());
    }
}