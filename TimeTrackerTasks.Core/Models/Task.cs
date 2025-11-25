using System;
using System.ComponentModel.DataAnnotations;

namespace TimeTrackerTasks.Core.Models
{
    public class TaskItem  // ← ИЗМЕНИТЬ НА TaskItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "Название задачи обязательно")]
        [MaxLength(100, ErrorMessage = "Название не может превышать 100 символов")]
        public string Title { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "Описание не может превышать 500 символов")]
        public string? Description { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? DueDate { get; set; }
        public bool IsCompleted { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Время должно быть положительным числом")]
        public int TimeSpentMinutes { get; set; }

        public bool Validate(out string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(Title))
            {
                errorMessage = "Название задачи обязательно";
                return false;
            }

            if (DueDate.HasValue && DueDate < CreatedDate)
            {
                errorMessage = "Срок выполнения не может быть раньше даты создания";
                return false;
            }

            if (TimeSpentMinutes < 0)
            {
                errorMessage = "Затраченное время не может быть отрицательным";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }
    }
}