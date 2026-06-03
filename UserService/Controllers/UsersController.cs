using FluentValidation;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserService.Data;
using UserService.DataTransferObjects;
using UserService.Models;

namespace UserService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly UserDbContext _context;
    private readonly IValidator<UserRequest> _validator;

    public UsersController(UserDbContext context, IValidator<UserRequest> validator)
    {
        _context = context;
        _validator = validator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _context.Users.ToListAsync();
        return Ok(users.Adapt<List<UserResponse>>());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user is null) return NotFound();
        return Ok(user.Adapt<UserResponse>());
    }

    [HttpPost]
    public async Task<IActionResult> Create(UserRequest request)
    {
        var result = await _validator.ValidateAsync(request);
        if (!result.IsValid)
            return BadRequest(result.Errors.Select(e => e.ErrorMessage));

        var user = request.Adapt<User>();
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user.Adapt<UserResponse>());
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UserRequest request)
    {
        var result = await _validator.ValidateAsync(request);
        if (!result.IsValid)
            return BadRequest(result.Errors.Select(e => e.ErrorMessage));

        var user = await _context.Users.FindAsync(id);
        if (user is null) return NotFound();

        request.Adapt(user);
        await _context.SaveChangesAsync();
        return Ok(user.Adapt<UserResponse>());
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user is null) return NotFound();

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}