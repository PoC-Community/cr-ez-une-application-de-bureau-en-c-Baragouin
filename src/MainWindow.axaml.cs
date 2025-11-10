using System;
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

    public MainWindow()
    {
        InitializeComponent();
        TaskList.ItemsSource = _tasks;

        AddButton.Click += OnAddClick;
        DeleteButton.Click += OnDeleteClick;
        SaveButton.Click += OnSaveClick;
    }

    private void OnAddClick(object? sender, RoutedEventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(TaskInput.Text))
        {
            _tasks.Add(new TaskItem { Title = TaskInput.Text});
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
            var dataDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");
            var dataSerialized = JsonSerializer.Serialize(_tasks, new JsonSerializerOptions { WriteIndented = true });
            var dataFile = Path.Combine(dataDirectory, "tasks.json");

            if (!Directory.Exists(dataDirectory))
                Directory.CreateDirectory(dataDirectory);
            await File.WriteAllTextAsync(dataFile, dataSerialized);
            
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
}
