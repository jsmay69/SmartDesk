using System;

namespace SmartDesk.Application.DTOs;

public class TodoItemDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? DueDate { get; set; }
    public string Priority { get; set; } = "Normal";
    public bool IsCompleted { get; set; }
}
