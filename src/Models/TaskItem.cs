using System;

namespace TodoListApp.Models;

public class TaskItem
{
    public string Title { get; set; } = string.Empty;
    public bool IsCompleted { get; set; } = false;
    public Guid Id { get; } = Guid.NewGuid();
}
