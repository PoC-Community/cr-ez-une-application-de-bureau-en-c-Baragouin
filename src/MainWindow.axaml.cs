using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;
using MsBox.Avalonia;
using System.Collections.ObjectModel;
using System.IO;
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

        if (!Directory.Exists(_dataDirectoryPath))
            Directory.CreateDirectory(_dataDirectoryPath);
        _dataFilePath = Path.Combine(_dataDirectoryPath, "tasks.json");
        LoadTasks();
    }

    private void OnAddClick(object? sender, RoutedEventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(TaskInput.Text))
        {
            _tasks.Add(new TaskItem { Title = TaskInput.Text });
            TaskInput.Text = string.Empty;
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