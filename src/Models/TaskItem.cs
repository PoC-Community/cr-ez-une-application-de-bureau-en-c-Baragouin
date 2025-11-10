using System;
using System.Collections.Generic;

namespace TodoListApp.Models;

public class TaskItem
{
    public string Title { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public List<string> Tags { get; set; } = [];
}
