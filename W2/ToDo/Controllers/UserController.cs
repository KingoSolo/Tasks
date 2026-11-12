using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDo.DTOs;
using ToDo.Services;

namespace ToDo.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    // GET: /User/all-users?page=1
    [HttpGet("all-users")]
    public async Task<IActionResult> GetAllToDos(int page, int pageSize){
        if (page < 1)
        {
            return BadRequest("Page number must be greater than 0");
        }
        var todos = await _userService.GetAllToDoAsync(page,pageSize);
        return Ok(todos);
    }

    // GET: /User/1
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetTodoById(int id)
    {
        var todo = await _userService.GetToDoByIdAsync(id);
        if(todo == null) return NotFound("result not found");
        return Ok(todo);
    }

    // GET: /User/example@gmail.com
    [HttpGet("{email}")]
    public async Task<IActionResult> GetTodoByEmail(string email)
    {
        var todo = await _userService.GetToDoByEmailAsync(email);
        if(todo == null) return NotFound("result not found");
        return Ok(todo);
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchTodos(string? term, DateTime? date)
    {
        var todo = await _userService.SearchToDoAsync(term,date);
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
        var createdToDo = await _userService.AddToDoAsync(request);
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

        var updatedTodo = await _userService.UpdateToDoAsync(id, request);
        if (updatedTodo == null) return NotFound("Todo not found.");

        return Ok(updatedTodo);
    }

    [HttpDelete("delete/{id:int}")]
    public async Task<IActionResult> DeleteTodo(int id)
    {
        var deleted = await _userService.DeleteToDoAsync(id);
        if (!deleted) return NotFound("Todo not found.");

        return Ok("Todo deleted successfully.");
    }

    [HttpDelete("delete/email/{email}")]
    public async Task<IActionResult> DeleteTodoByEmail(string email)
    {
      

     
        var deleted = await _userService.DeleteToDoByEmailAsync(email);
        if (!deleted) return NotFound("Todo not found.");

        return Ok("Todo deleted successfully.");
    }
}