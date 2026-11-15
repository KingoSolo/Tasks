using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDo.DTOs;
using ToDo.Services;

namespace ToDo.Controllers;

[ApiController]
[Route("api/todos")]
[Authorize]
public class TodoController : ControllerBase
{
    private readonly ITodoService _todoService;

    public TodoController(ITodoService todoService)
    {
        _todoService = todoService;
    }

    private string currentUserId =>
        User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

    // GET: /User/all-users?page=1
    [HttpGet("all-todos")]
    public async Task<IActionResult> GetAllToDos(int page, int pageSize){
        if (page < 1)
        {
            return BadRequest("Page number must be greater than 0");
        }
        var todos = await _todoService.GetAllToDoAsync(page,pageSize,currentUserId);
        return Ok(todos);
    }

    // GET: /User/1
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetTodoById(int id)
    {
        var todo = await _todoService.GetToDoByIdAsync(id,currentUserId);
        if(todo == null) return NotFound("result not found");
        return Ok(todo);
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchTodos(string? term, DateTime? date)
    {
        var todo = await _todoService.SearchToDoAsync(term,date,currentUserId);
        return Ok(todo);
    }

    // POST: /User/add-new
    [HttpPost("add-new")]
    public async Task<IActionResult> AddTodo(CreateTodoDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            return BadRequest("Todo title is required");
        }
        var createdToDo = await _todoService.AddToDoAsync(request,currentUserId);
        return CreatedAtAction(
            nameof(GetTodoById),
            new { id = createdToDo.Id },
            createdToDo
        );
    }

    [HttpPut("update/{id:int}")]
    public async Task<IActionResult> UpdateTodo(int id, UpdateTodoDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            return BadRequest("Todo title is required.");
        }

        var updatedTodo = await _todoService.UpdateToDoAsync(id, request,currentUserId);
        if (updatedTodo == null) return NotFound("Todo not found.");

        return Ok(updatedTodo);
    }

    [HttpDelete("delete/{id:int}")]
    public async Task<IActionResult> DeleteTodo(int id)
    {
        var deleted = await _todoService.DeleteToDoAsync(id,currentUserId);
        if (!deleted) return NotFound("Todo not found.");

        return Ok("Todo deleted successfully.");
    }

}