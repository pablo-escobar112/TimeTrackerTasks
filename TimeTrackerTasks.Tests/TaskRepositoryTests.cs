using Microsoft.EntityFrameworkCore;
using TimeTrackerTasks.Core.Models;
using TimeTrackerTasks.Data.Data;
using TimeTrackerTasks.Data.Repositories;
using Xunit;

namespace TimeTrackerTasks.Tests
{
    public class TaskRepositoryTests
    {
        private TaskDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<TaskDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new TaskDbContext(options);
        }

        [Fact]
        public async Task AddTaskAsync_ValidTask_TaskAddedToDatabase()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var repository = new TaskRepository(context);
            var task = new TaskItem { Title = "Test Task", Description = "Test Description" };

            // Act
            await repository.AddTaskAsync(task);

            // Assert
            var savedTask = await context.Tasks.FirstOrDefaultAsync();
            Assert.NotNull(savedTask);
            Assert.Equal("Test Task", savedTask.Title);
            Assert.Equal("Test Description", savedTask.Description);
        }

        [Fact]
        public async Task UpdateTaskAsync_ExistingTask_TaskUpdated()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var repository = new TaskRepository(context);
            var task = new TaskItem { Title = "Original Title" };
            await repository.AddTaskAsync(task);

            // Act
            task.Title = "Updated Title";
            task.IsCompleted = true;
            await repository.UpdateTaskAsync(task);

            // Assert
            var updatedTask = await repository.GetTaskByIdAsync(task.Id);
            Assert.NotNull(updatedTask);
            Assert.Equal("Updated Title", updatedTask.Title);
            Assert.True(updatedTask.IsCompleted);
        }

        [Fact]
        public async Task GetAllTasksAsync_WithTasks_ReturnsAllTasks()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var repository = new TaskRepository(context);

            var task1 = new TaskItem { Title = "Task 1" };
            var task2 = new TaskItem { Title = "Task 2" };
            await repository.AddTaskAsync(task1);
            await repository.AddTaskAsync(task2);

            // Act
            var tasks = await repository.GetAllTasksAsync();

            // Assert
            Assert.Equal(2, tasks.Count());
        }

        [Fact]
        public async Task DeleteTaskAsync_ExistingTask_TaskDeleted()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var repository = new TaskRepository(context);
            var task = new TaskItem { Title = "Task to delete" };
            await repository.AddTaskAsync(task);

            // Act
            await repository.DeleteTaskAsync(task.Id);

            // Assert
            var deletedTask = await repository.GetTaskByIdAsync(task.Id);
            Assert.Null(deletedTask);
        }

        [Fact]
        public async Task GetTaskByIdAsync_ExistingTask_ReturnsTask()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var repository = new TaskRepository(context);
            var task = new TaskItem { Title = "Specific Task" };
            await repository.AddTaskAsync(task);

            // Act
            var foundTask = await repository.GetTaskByIdAsync(task.Id);

            // Assert
            Assert.NotNull(foundTask);
            Assert.Equal(task.Id, foundTask.Id);
            Assert.Equal("Specific Task", foundTask.Title);
        }
    }
}