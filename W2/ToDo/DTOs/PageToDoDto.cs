namespace ToDo.DTOs;

public class PagedTodoResponseDto
{
    public List<ReturnTodoDto> Items { get; set; } = new();
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int ElementsOnPage { get; set; }
    public int TotalCount { get; set; }
}
