using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDo.Data;
using ToDo.DTOs;

using ToDo.Models;

namespace ToDo.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public UserController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: /User/all-users?page=1
    [HttpGet("all-users")]
    public async Task<IActionResult> GetAllTodos(int page = 1)
    {
        int pageSize = 5;

        if (page < 1)
        {
            return BadRequest("Page number must be greater than 0.");
        }

        var todos = await _context.Todos
            .OrderBy(todo => todo.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Ok(todos);
    }

    // GET: /User/1
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetTodoById(int id)
    {
        var todo = await _context.Todos.FindAsync(id);

        if (todo == null)
        {
            return NotFound("Todo not found.");
        }

        return Ok(todo);
    }

    // GET: /User/example@gmail.com
    [HttpGet("{email}")]
    public async Task<IActionResult> GetTodoByEmail(string email)
    {
        var todo = await _context.Todos
            .FirstOrDefaultAsync(todo => todo.Email == email);

        if (todo == null)
        {
            return NotFound("Todo not found.");
        }

        return Ok(todo);
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchTodos(string? term, DateTime? date)
    {
        var query = _context.Todos.AsQueryable();

        if (!string.IsNullOrWhiteSpace(term))
        {
            query = query.Where(todo =>
                todo.Title.Contains(term) ||
                todo.Description.Contains(term) ||
                todo.Email.Contains(term));
        }

        if (date.HasValue)
        {
            query = query.Where(todo =>
                todo.DueDate.Date == date.Value.Date);
        }

        var todos = await query.ToListAsync();

        return Ok(todos);
    }

    // POST: /User/add-new
    [HttpPost("add-new")]
    public async Task<IActionResult> AddTodo(CreateTodoDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            return BadRequest("Todo title is required.");
        }

        var todo = new Todo
        {
            Title = request.Title,
            Description = request.Description,
            Email = request.Email,
            DueDate = request.DueDate
        };

        _context.Todos.Add(todo);

        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetTodoById),
            new { id = todo.Id },
            todo
        );
    }

    // PUT: /User/update/1
    [HttpPut("update/{id:int}")]
    public async Task<IActionResult> UpdateTodo(int id, UpdateTodoDto request)
    {
        var todo = await _context.Todos.FindAsync(id);

        if (todo == null)
        {
            return NotFound("Todo not found.");
        }

        if (string.IsNullOrWhiteSpace(request.Title))
        {
            return BadRequest("Todo title is required.");
        }

        todo.Title = request.Title;
        todo.Description = request.Description;
        todo.Email = request.Email;
        todo.DueDate = request.DueDate;
        todo.IsCompleted = request.IsCompleted;

        await _context.SaveChangesAsync();

        return Ok(todo);
    }

    // DELETE: /User/delete/1
    [HttpDelete("delete/{id:int}")]
    public async Task<IActionResult> DeleteTodo(int id)
    {
        var todo = await _context.Todos.FindAsync(id);

        if (todo == null)
        {
            return NotFound("Todo not found.");
        }

        _context.Todos.Remove(todo);

        await _context.SaveChangesAsync();

        return Ok("Todo deleted successfully.");
    }

    // DELETE: /User/delete/email/example@gmail.com
    [HttpDelete("delete/email/{email}")]
    public async Task<IActionResult> DeleteTodoByEmail(string email)
    {
        var todo = await _context.Todos
            .FirstOrDefaultAsync(todo => todo.Email == email);

        if (todo == null)
        {
            return NotFound("Todo not found.");
        }

        _context.Todos.Remove(todo);

        await _context.SaveChangesAsync();

        return Ok("Todo deleted successfully.");
    }
}