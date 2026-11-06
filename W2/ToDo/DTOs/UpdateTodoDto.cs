namespace ToDo.DTOs;

public class UpdateTodoDto
{
    public string Title { get; set; } = "";

    public string Description { get; set; } = "";

    public string Email { get; set; } = "";

    public DateTime DueDate { get; set; }

    public bool IsCompleted { get; set; }
}