using System.Windows;
using TimeTrackerTasks.Core.Models;

namespace TimeTrackerTasks.WPF.Views
{
    public partial class AddEditTaskWindow : Window
    {
        public TaskItem? Task { get; private set; }

        public AddEditTaskWindow(TaskItem? existingTask = null)
        {
            InitializeComponent();

            if (existingTask != null)
            {
                Task = existingTask;
                DataContext = Task;
                Title = "Редактировать задачу";
            }
            else
            {
                Task = new TaskItem();
                DataContext = Task;
                Title = "Добавить задачу";
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Добавим проверку на null
            if (Task == null) return;

            if (string.IsNullOrWhiteSpace(Task.Title))
            {
                MessageBox.Show("Название задачи обязательно!", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Добавим проверку на null для DueDate
            if (Task.DueDate.HasValue && Task.DueDate.Value < Task.CreatedDate)
            {
                MessageBox.Show("Срок выполнения не может быть раньше даты создания!", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error); // ← ИСПРАВИТЬ аргументы
                return;
            }

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}