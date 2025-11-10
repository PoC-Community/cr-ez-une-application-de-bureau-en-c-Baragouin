using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;
using MsBox.Avalonia;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using TodoListApp.Models;

namespace TodoListApp;

public partial class MainWindow : Window
{
    private ObservableCollection<TaskItem> _tasks = new();
    private readonly string _dataDirectoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");
    private readonly string _dataFilePath;

    public MainWindow()
    {
        InitializeComponent();
        TaskList.ItemsSource = _tasks;

        AddButton.Click += OnAddClick;
        DeleteButton.Click += OnDeleteClick;
        SaveButton.Click += OnSaveClick;
        TagFilter.TextChanged += OnTagFilterChanged;
        ClearFilterButton.Click += OnClearFilterClick;

        if (!Directory.Exists(_dataDirectoryPath))
            Directory.CreateDirectory(_dataDirectoryPath);
        _dataFilePath = Path.Combine(_dataDirectoryPath, "tasks.json");
        LoadTasks();
    }

    private void OnAddClick(object? sender, RoutedEventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(TaskInput.Text))
        {
            var tags = TagsInput.Text is { Length: > 0 } text
                ? text.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).ToList()
                : [];

            _tasks.Add(new TaskItem { Title = TaskInput.Text, Tags = tags });
            TaskInput.Text = string.Empty;
            TagsInput.Text = string.Empty;
        }
    }

    private void OnDeleteClick(object? sender, RoutedEventArgs e)
    {
        if (TaskList.SelectedItem is TaskItem selected)
        {
            _tasks.Remove(selected);
        }
    }

    private async void OnSaveClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            var dataSerialized = JsonSerializer.Serialize(_tasks, new JsonSerializerOptions { WriteIndented = true });

            await File.WriteAllTextAsync(_dataFilePath, dataSerialized);

            var successBox = MessageBoxManager
                .GetMessageBoxStandard("Success", "Your tasks have been saved successfully!");

            await successBox.ShowAsync();
        }
        catch (Exception ex)
        {
            var errorBox = MessageBoxManager
                .GetMessageBoxStandard("Error", "Failed to save tasks:\n" + ex.Message);

            await errorBox.ShowAsync();
        }
    }

    private void OnTagFilterChanged(object? sender, RoutedEventArgs e)
    {
        var filteredTasks = _tasks;

        if (!string.IsNullOrWhiteSpace(TagFilter.Text))
        {
            filteredTasks =
                new ObservableCollection<TaskItem>(_tasks
                    .Where(task => task.Tags.Any(tag => tag.Contains(TagFilter.Text))));
        }

        TaskList.ItemsSource = filteredTasks;
    }

    private void OnClearFilterClick(object? sender, RoutedEventArgs e)
    {
        TagFilter.Clear();
    }

    private void LoadTasks()
    {
        _tasks.Clear();

        try
        {
            var rawData = File.ReadAllText(_dataFilePath);
            var tasks = JsonSerializer.Deserialize<List<TaskItem>>(rawData);

            if (tasks is null)
            {
                Console.WriteLine("No tasks found in file");
                return;
            }

            foreach (var task in tasks)
                _tasks.Add(task);
            Console.WriteLine($"Load successful: {tasks.Count} task(s) found");
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"Corrupted JSON file: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error while loading: {ex.Message}");
        }
    }
}