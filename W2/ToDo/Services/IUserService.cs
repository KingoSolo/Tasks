using ToDo.DTOs;
namespace ToDo.Services;

public interface IUserService
{
    
    Task<ReturnTodoDto?>GetToDoByIdAsync(int id);
    Task<ReturnTodoDto?>GetToDoByEmailAsync(string email);
    Task<List<ReturnTodoDto>>SearchToDoAsync(string? term, DateTime? date);
    Task<ReturnTodoDto>AddToDoAsync(CreateTodoDto request);
    Task<ReturnTodoDto?>UpdateToDoAsync(int id,UpdateTodoDto request);
    Task<bool>DeleteToDoAsync(int id);
    Task<bool>DeleteToDoByEmailAsync(string email);
    Task<PagedTodoResponseDto> GetAllToDoAsync(int page, int pageSize);

}