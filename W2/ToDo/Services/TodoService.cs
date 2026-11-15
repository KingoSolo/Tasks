using Microsoft.EntityFrameworkCore;
using ToDo.DTOs;
using ToDo.Services;
using ToDo.Models;
using ToDo.Data;

namespace ToDo.Services;

public class TodoService : ITodoService
{
    private readonly ApplicationDbContext _context;
    public TodoService(ApplicationDbContext context)
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

    public async Task<PagedTodoResponseDto> GetAllToDoAsync(int page, int pageSize, string userId)
    {
        var query = _context.Todos
            .Where(todo =>  todo.UserId == userId);

        int totalCount = await query.CountAsync();
        var todos =  await query
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

    public async Task<ReturnTodoDto?>GetToDoByIdAsync(int Id,string userId)
    {
        var todo = await _context.Todos
            .FirstOrDefaultAsync(todo =>
                todo.Id == Id && todo.UserId == userId);        
        return todo == null ? null:MapToDto(todo);
    }

    public async Task<List<ReturnTodoDto>> SearchToDoAsync(string? term, DateTime? date,string userId)
    {
        var query = _context.Todos
                .Where(todo => todo.UserId == userId)
                .AsQueryable();

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

    public async Task<ReturnTodoDto> AddToDoAsync(CreateTodoDto request,string userId)
    {
        var todo = new Todo
        {
            Title = request.Title,
            Description = request.Description,
            DueDate = request.DueDate,
            UserId = userId
        };

        _context.Todos.Add(todo);
        await _context.SaveChangesAsync();

        return MapToDto(todo);
    }

    public async Task<ReturnTodoDto?> UpdateToDoAsync(int id, UpdateTodoDto request,string userId)
    {
        var todo = await _context.Todos.FirstOrDefaultAsync(
            todo => todo.Id == id && todo.UserId == userId
        );
        if (todo == null) return null;

        todo.Title = request.Title;
        todo.Description = request.Description;
        todo.Email = request.Email;
        todo.DueDate = request.DueDate;
        todo.IsCompleted = request.IsCompleted;

        await _context.SaveChangesAsync();
         return MapToDto(todo);
    }

    public async Task<bool> DeleteToDoAsync(int id,string userId)
    {
        var todo = await _context.Todos.FirstOrDefaultAsync(
            todo => todo.Id == id && todo.UserId == userId
        );
        if (todo == null) return false;

        _context.Todos.Remove(todo);
        await _context.SaveChangesAsync();
        return true;
    }
    

}