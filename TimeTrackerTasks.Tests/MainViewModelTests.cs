using Moq;
using TimeTrackerTasks.Core.Interfaces;
using TimeTrackerTasks.Core.Models;
using TimeTrackerTasks.WPF.ViewModels;
using Xunit;

namespace TimeTrackerTasks.Tests
{
    public class MainViewModelTests
    {
        private Mock<ITaskRepository> CreateMockRepository()
        {
            var mockRepository = new Mock<ITaskRepository>();
            var sampleTasks = new List<TaskItem>
            {
                new TaskItem { Id = Guid.NewGuid(), Title = "Task 1", IsCompleted = false },
                new TaskItem { Id = Guid.NewGuid(), Title = "Task 2", IsCompleted = true },
                new TaskItem { Id = Guid.NewGuid(), Title = "Test Task", IsCompleted = false }
            };

            mockRepository.Setup(repo => repo.GetAllTasksAsync())
                         .ReturnsAsync(sampleTasks);

            return mockRepository;
        }

        [Fact]
        public void AddTimeCommand_ValidTimeAndSelectedTask_TimeAddedSuccessfully()
        {
            // Arrange
            var mockRepository = CreateMockRepository();
            var viewModel = new MainViewModel(mockRepository.Object);
            var task = new TaskItem { Title = "Test Task", TimeSpentMinutes = 10 };
            viewModel.SelectedTask = task;
            viewModel.TimeToAdd = "15";

            // Act
            viewModel.AddTimeCommand.Execute(null);

            // Assert
            Assert.Equal(25, task.TimeSpentMinutes);
            mockRepository.Verify(repo => repo.UpdateTaskAsync(task), Times.Once);
        }

        [Fact]
        public void AddTimeCommand_NoTaskSelected_CommandCannotExecute()
        {
            // Arrange
            var mockRepository = CreateMockRepository();
            var viewModel = new MainViewModel(mockRepository.Object);
            viewModel.SelectedTask = null;
            viewModel.TimeToAdd = "15";

            // Act & Assert
            Assert.False(viewModel.AddTimeCommand.CanExecute(null));
        }

        [Fact]
        public void AddTimeCommand_InvalidTime_CommandCannotExecute()
        {
            // Arrange
            var mockRepository = CreateMockRepository();
            var viewModel = new MainViewModel(mockRepository.Object);
            viewModel.SelectedTask = new TaskItem { Title = "Test" };
            viewModel.TimeToAdd = "invalid";

            // Act & Assert
            Assert.False(viewModel.AddTimeCommand.CanExecute(null));
        }

        [Fact]
        public void AddTimeCommand_NegativeTime_CommandCannotExecute()
        {
            // Arrange
            var mockRepository = CreateMockRepository();
            var viewModel = new MainViewModel(mockRepository.Object);
            viewModel.SelectedTask = new TaskItem { Title = "Test" };
            viewModel.TimeToAdd = "-5";

            // Act & Assert
            Assert.False(viewModel.AddTimeCommand.CanExecute(null));
        }

        [Fact]
        public void FilterTasks_SearchText_FiltersCorrectly()
        {
            // Arrange
            var mockRepository = CreateMockRepository();
            var viewModel = new MainViewModel(mockRepository.Object);

            // Ждем загрузки задач
            viewModel.LoadTasksCommand.Execute(null);

            // Act
            viewModel.SearchText = "Test";

            // Assert
            var filteredTasks = viewModel.FilteredTasks;
            Assert.Single(filteredTasks);
            Assert.All(filteredTasks, task => task.Title.Contains("Test"));
        }

        [Fact]
        public void FilterTasks_ActiveFilter_ShowsOnlyActiveTasks()
        {
            // Arrange
            var mockRepository = CreateMockRepository();
            var viewModel = new MainViewModel(mockRepository.Object);
            viewModel.LoadTasksCommand.Execute(null);

            // Act
            viewModel.CurrentFilter = Core.Enums.TaskFilter.Active;

            // Assert
            var filteredTasks = viewModel.FilteredTasks;
            Assert.All(filteredTasks, task => Assert.False(task.IsCompleted));
        }

        [Fact]
        public void FilterTasks_CompletedFilter_ShowsOnlyCompletedTasks()
        {
            // Arrange
            var mockRepository = CreateMockRepository();
            var viewModel = new MainViewModel(mockRepository.Object);
            viewModel.LoadTasksCommand.Execute(null);

            // Act
            viewModel.CurrentFilter = Core.Enums.TaskFilter.Completed;

            // Assert
            var filteredTasks = viewModel.FilteredTasks;
            Assert.All(filteredTasks, task => Assert.True(task.IsCompleted));
        }

        [Fact]
        public void EditTaskCommand_NoTaskSelected_CommandCannotExecute()
        {
            // Arrange
            var mockRepository = CreateMockRepository();
            var viewModel = new MainViewModel(mockRepository.Object);
            viewModel.SelectedTask = null;

            // Act & Assert
            Assert.False(viewModel.EditTaskCommand.CanExecute(null));
        }

        [Fact]
        public void DeleteTaskCommand_NoTaskSelected_CommandCannotExecute()
        {
            // Arrange
            var mockRepository = CreateMockRepository();
            var viewModel = new MainViewModel(mockRepository.Object);
            viewModel.SelectedTask = null;

            // Act & Assert
            Assert.False(viewModel.DeleteTaskCommand.CanExecute(null));
        }

        [Fact]
        public void MarkAsCompletedCommand_CompletedTask_CommandCannotExecute()
        {
            // Arrange
            var mockRepository = CreateMockRepository();
            var viewModel = new MainViewModel(mockRepository.Object);
            var completedTask = new TaskItem { Title = "Completed", IsCompleted = true };
            viewModel.SelectedTask = completedTask;

            // Act & Assert
            Assert.False(viewModel.MarkAsCompletedCommand.CanExecute(null));
        }
    }
}