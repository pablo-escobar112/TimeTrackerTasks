using TimeTrackerTasks.Core.Models;

namespace TimeTrackerTasks.Core.Interfaces
{
    public interface ITaskRepository
    {
        System.Threading.Tasks.Task<System.Collections.Generic.IEnumerable<TaskItem>> GetAllTasksAsync();
        System.Threading.Tasks.Task<TaskItem?> GetTaskByIdAsync(Guid id);
        System.Threading.Tasks.Task AddTaskAsync(TaskItem task);
        System.Threading.Tasks.Task UpdateTaskAsync(TaskItem task);
        System.Threading.Tasks.Task DeleteTaskAsync(Guid id);
    }
}