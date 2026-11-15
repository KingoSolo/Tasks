using ToDo.DTOs;
namespace ToDo.Services;

public interface ITodoService
{
    
    Task<ReturnTodoDto?>GetToDoByIdAsync(int id,string userId);
    Task<List<ReturnTodoDto>>SearchToDoAsync(string? term, DateTime? date,string userId);
    Task<ReturnTodoDto>AddToDoAsync(CreateTodoDto request,string userId);
    Task<ReturnTodoDto?>UpdateToDoAsync(int id,UpdateTodoDto request,string userId);
    Task<bool>DeleteToDoAsync(int id,string userId);
    Task<PagedTodoResponseDto> GetAllToDoAsync(int page, int pageSize,string userId);

}