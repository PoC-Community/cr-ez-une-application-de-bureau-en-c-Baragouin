using System;
using System.Collections.Generic;

namespace TodoListApp.Models;

public class TaskItem
{
    public Guid Id { get; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public bool IsCompleted { get; set; } = false;
    public List<string> Tags { get; set; } = new();
}
