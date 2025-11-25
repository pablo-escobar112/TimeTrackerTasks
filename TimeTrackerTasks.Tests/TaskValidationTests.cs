using TimeTrackerTasks.Core.Models;
using Xunit;

namespace TimeTrackerTasks.Tests
{
    public class TaskValidationTests
    {
        [Fact]
        public void Validate_EmptyTitle_ReturnsFalse()
        {
            // Arrange
            var task = new TaskItem { Title = "" };

            // Act
            var isValid = task.Validate(out var errorMessage);

            // Assert
            Assert.False(isValid);
            Assert.Contains("обязательно", errorMessage);
        }

        [Fact]
        public void Validate_WhitespaceTitle_ReturnsFalse()
        {
            // Arrange
            var task = new TaskItem { Title = "   " };

            // Act
            var isValid = task.Validate(out var errorMessage);

            // Assert
            Assert.False(isValid);
            Assert.Contains("обязательно", errorMessage);
        }

        [Fact]
        public void Validate_DueDateBeforeCreatedDate_ReturnsFalse()
        {
            // Arrange
            var task = new TaskItem
            {
                Title = "Test Task",
                CreatedDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(-1)
            };

            // Act
            var isValid = task.Validate(out var errorMessage);

            // Assert
            Assert.False(isValid);
            Assert.Contains("раньше даты создания", errorMessage);
        }

        [Fact]
        public void Validate_NegativeTimeSpent_ReturnsFalse()
        {
            // Arrange
            var task = new TaskItem
            {
                Title = "Test Task",
                TimeSpentMinutes = -10
            };

            // Act
            var isValid = task.Validate(out var errorMessage);

            // Assert
            Assert.False(isValid);
            Assert.Contains("отрицательным", errorMessage);
        }

        [Fact]
        public void Validate_ValidTask_ReturnsTrue()
        {
            // Arrange
            var task = new TaskItem
            {
                Title = "Valid Task",
                DueDate = DateTime.Now.AddDays(1),
                TimeSpentMinutes = 60
            };

            // Act
            var isValid = task.Validate(out var errorMessage);

            // Assert
            Assert.True(isValid);
            Assert.Empty(errorMessage);
        }

        [Fact]
        public void Validate_ValidTaskWithoutDueDate_ReturnsTrue()
        {
            // Arrange
            var task = new TaskItem
            {
                Title = "Valid Task",
                DueDate = null,
                TimeSpentMinutes = 0
            };

            // Act
            var isValid = task.Validate(out var errorMessage);

            // Assert
            Assert.True(isValid);
            Assert.Empty(errorMessage);
        }
    }
}