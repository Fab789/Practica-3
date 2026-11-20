using PAW.MinimalAPI.Models;

namespace PAW.MinimalAPI.Services;

public class TaskService : ITaskService
{
    private readonly List<TaskItem> _tasks = new()
    {
        new() { Id = 1, Title = "Estudiar Minimal API", Description = "Revisar tutorial de Microsoft", DueDate = DateTime.Today.AddDays(2) },
        new() { Id = 2, Title = "Entregar ejercicio PAW", Description = "Subir proyecto", DueDate = DateTime.Today.AddDays(5), IsCompleted = true },
        new() { Id = 3, Title = "Practicar Dependency Injection", Description = "Registrar servicios en Program.cs" }
    };
    private int _nextId = 4;
    private readonly object _lock = new();

    public Task<IEnumerable<TaskItem>> GetAllAsync()
    {
        lock (_lock) return Task.FromResult<IEnumerable<TaskItem>>(_tasks.ToList());
    }

    public Task<TaskItem?> GetByIdAsync(int id)
    {
        lock (_lock) return Task.FromResult(_tasks.FirstOrDefault(t => t.Id == id));
    }

    public Task<TaskItem> CreateAsync(TaskItem task)
    {
        lock (_lock)
        {
            task.Id = _nextId++;
            _tasks.Add(task);
            return Task.FromResult(task);
        }
    }

    public Task<bool> UpdateAsync(int id, TaskItem task)
    {
        lock (_lock)
        {
            var existing = _tasks.FirstOrDefault(t => t.Id == id);
            if (existing is null) return Task.FromResult(false);
            existing.Title = task.Title;
            existing.Description = task.Description;
            existing.DueDate = task.DueDate;
            existing.IsCompleted = task.IsCompleted;
            return Task.FromResult(true);
        }
    }

    public Task<bool> DeleteAsync(int id)
    {
        lock (_lock)
        {
            var existing = _tasks.FirstOrDefault(t => t.Id == id);
            if (existing is null) return Task.FromResult(false);
            _tasks.Remove(existing);
            return Task.FromResult(true);
        }
    }
}
