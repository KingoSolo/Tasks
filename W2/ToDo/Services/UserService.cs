using Microsoft.EntityFrameworkCore;
using ToDo.DTOs;
using ToDo.Services;
using ToDo.Models;
using ToDo.Data;

namespace ToDo.Services;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;
    public UserService(ApplicationDbContext context)
    {
        _context = context;
    }

    private static ReturnTodoDto MapToDto(Todo todo)
    {
        return new ReturnTodoDto
        {
            Id = todo.Id,
            Title = todo.Title,
            Description = todo.Description,
            DueDate = todo.DueDate,
            IsCompleted = todo.IsCompleted,
            CreatedAt = todo.CreatedAt
        };
    }

    public async Task<PagedTodoResponseDto> GetAllToDoAsync(int page, int pageSize)
    {
        int totalCount = await _context.Todos.CountAsync();
        var todos =  await _context.Todos
            .OrderBy(todo => todo.Id)
            .Skip((page-1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        var todoDtos = todos.Select(MapToDto).ToList();

   
    return new PagedTodoResponseDto
    {
        Items = todoDtos,
        CurrentPage = page,
        PageSize = pageSize,
        ElementsOnPage = todoDtos.Count,
        TotalCount = totalCount
    };
    }

    public async Task<ReturnTodoDto?>GetToDoByIdAsync(int id)
    {
        var todo = await _context.Todos.FindAsync(id);
        return todo == null ? null:MapToDto(todo);
    }

    public async Task<ReturnTodoDto?> GetToDoByEmailAsync(string email)
    {
        var todo = await _context.Todos
            .FirstOrDefaultAsync(t => t.Email == email);
        return todo == null ? null : MapToDto(todo);
    }

    public async Task<List<ReturnTodoDto>> SearchToDoAsync(string? term, DateTime? date)
    {
        var query = _context.Todos.AsQueryable();

        if (!string.IsNullOrWhiteSpace(term))
        {
            query = query.Where(todo =>
                todo.Title.ToLower().Contains(term.ToLower()) ||
                todo.Description.Contains(term) ||
                todo.Email.Contains(term));
        }

        if (date.HasValue)
        {
            query = query.Where(todo => todo.DueDate.Date == date.Value.Date);
        }
        var matchingTodos = await query.ToListAsync();

        return matchingTodos.Select(todo => MapToDto(todo)).ToList();
    }

    public async Task<ReturnTodoDto> AddToDoAsync(CreateTodoDto request)
    {
        var todo = new Todo
        {
            Title = request.Title,
            Description = request.Description,
            Email = request.Email,
            DueDate = request.DueDate
        };

        _context.Todos.Add(todo);
        await _context.SaveChangesAsync();

        return MapToDto(todo);
    }

    public async Task<ReturnTodoDto?> UpdateToDoAsync(int id, UpdateTodoDto request)
    {
        var todo = await _context.Todos.FindAsync(id);
        if (todo == null) return null;

        todo.Title = request.Title;
        todo.Description = request.Description;
        todo.Email = request.Email;
        todo.DueDate = request.DueDate;
        todo.IsCompleted = request.IsCompleted;

        await _context.SaveChangesAsync();
         return MapToDto(todo);
    }

    public async Task<bool> DeleteToDoAsync(int id)
    {
        var todo = await _context.Todos.FindAsync(id);
        if (todo == null) return false;

        _context.Todos.Remove(todo);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteToDoByEmailAsync(string email)
    {
        var todo = await _context.Todos.FirstOrDefaultAsync(t => t.Email == email);
        if (todo == null) return false;

        _context.Todos.Remove(todo);
        await _context.SaveChangesAsync();
        return true;
    }

    

}