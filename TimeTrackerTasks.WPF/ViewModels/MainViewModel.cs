using System.Collections.ObjectModel;
using System.Windows.Input;
using TimeTrackerTasks.Core.Enums;
using TimeTrackerTasks.Core.Interfaces;
using TimeTrackerTasks.Core.Models;
using TimeTrackerTasks.WPF.Utilities;
using TimeTrackerTasks.WPF.Views;

namespace TimeTrackerTasks.WPF.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly ITaskRepository _taskRepository;
        private ObservableCollection<TaskItem> _tasks;  // ← ИЗМЕНИТЬ
        private TaskItem? _selectedTask;  // ← ИЗМЕНИТЬ
        private string _searchText = string.Empty;
        private TaskFilter _currentFilter = TaskFilter.All;
        private string _timeToAdd = string.Empty;

        public MainViewModel(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
            _tasks = new ObservableCollection<TaskItem>();  // ← ИЗМЕНИТЬ

            LoadTasksCommand = new RelayCommand(async _ => await LoadTasks());
            AddTaskCommand = new RelayCommand(_ => ShowAddTaskWindow());
            EditTaskCommand = new RelayCommand(_ => ShowEditTaskWindow(), _ => SelectedTask != null);
            DeleteTaskCommand = new RelayCommand(async _ => await DeleteTask(), _ => SelectedTask != null);
            MarkAsCompletedCommand = new RelayCommand(async _ => await MarkAsCompleted(), _ => SelectedTask != null && !SelectedTask.IsCompleted);
            AddTimeCommand = new RelayCommand(_ => AddTime(), _ => SelectedTask != null && int.TryParse(TimeToAdd, out int minutes) && minutes > 0);

            LoadTasksCommand.Execute(null);
        }

        public ObservableCollection<TaskItem> Tasks  // ← ИЗМЕНИТЬ
        {
            get => _tasks;
            set => SetField(ref _tasks, value);
        }

        public TaskItem? SelectedTask  // ← ИЗМЕНИТЬ
        {
            get => _selectedTask;
            set => SetField(ref _selectedTask, value);
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                SetField(ref _searchText, value);
                OnPropertyChanged(nameof(FilteredTasks));
            }
        }

        public TaskFilter CurrentFilter
        {
            get => _currentFilter;
            set
            {
                SetField(ref _currentFilter, value);
                OnPropertyChanged(nameof(FilteredTasks));
            }
        }

        public string TimeToAdd
        {
            get => _timeToAdd;
            set
            {
                SetField(ref _timeToAdd, value);
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public System.Collections.Generic.IEnumerable<TaskFilter> Filters => System.Linq.Enumerable.Cast<TaskFilter>(System.Enum.GetValues(typeof(TaskFilter)));

        public System.Collections.Generic.IEnumerable<TaskItem> FilteredTasks  // ← ИЗМЕНИТЬ
        {
            get
            {
                var query = Tasks.AsEnumerable();

                query = CurrentFilter switch
                {
                    TaskFilter.Active => query.Where(t => !t.IsCompleted),
                    TaskFilter.Completed => query.Where(t => t.IsCompleted),
                    _ => query
                };

                if (!string.IsNullOrWhiteSpace(SearchText))
                {
                    query = query.Where(t => t.Title.Contains(SearchText, System.StringComparison.OrdinalIgnoreCase));
                }

                return query;
            }
        }

        public ICommand LoadTasksCommand { get; }
        public ICommand AddTaskCommand { get; }
        public ICommand EditTaskCommand { get; }
        public ICommand DeleteTaskCommand { get; }
        public ICommand MarkAsCompletedCommand { get; }
        public ICommand AddTimeCommand { get; }

        private async System.Threading.Tasks.Task LoadTasks()  // ← ДОБАВИТЬ возвращаемое значение
        {
            try
            {
                var tasks = await _taskRepository.GetAllTasksAsync();
                Tasks = new ObservableCollection<TaskItem>(tasks);
                OnPropertyChanged(nameof(FilteredTasks));
            }
            catch (System.Exception ex)
            {
                System.Windows.MessageBox.Show($"Ошибка загрузки задач: {ex.Message}", "Ошибка");
            }
        }

        private void ShowAddTaskWindow()
        {
            var addWindow = new AddEditTaskWindow();
            if (addWindow.ShowDialog() == true && addWindow.Task != null)
            {
                _ = _taskRepository.AddTaskAsync(addWindow.Task);
                LoadTasksCommand.Execute(null);
            }
        }

        private void ShowEditTaskWindow()
        {
            if (SelectedTask == null) return;

            var editWindow = new AddEditTaskWindow(SelectedTask);
            if (editWindow.ShowDialog() == true)
            {
                _ = _taskRepository.UpdateTaskAsync(SelectedTask);
                LoadTasksCommand.Execute(null);
            }
        }

        private async System.Threading.Tasks.Task DeleteTask()  // ← ДОБАВИТЬ возвращаемое значение
        {
            if (SelectedTask != null)
            {
                var result = System.Windows.MessageBox.Show($"Удалить задачу '{SelectedTask.Title}'?", "Подтверждение",
                    System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Question);

                if (result == System.Windows.MessageBoxResult.Yes)
                {
                    await _taskRepository.DeleteTaskAsync(SelectedTask.Id);
                    await LoadTasks();
                }
            }
        }

        private async System.Threading.Tasks.Task MarkAsCompleted()  // ← ДОБАВИТЬ возвращаемое значение
        {
            if (SelectedTask != null)
            {
                SelectedTask.IsCompleted = true;
                await _taskRepository.UpdateTaskAsync(SelectedTask);
                await LoadTasks();
            }
        }

        private async void AddTime()  // ← ИЗМЕНИТЬ НА async void
        {
            if (SelectedTask != null && int.TryParse(TimeToAdd, out int minutes) && minutes > 0)
            {
                SelectedTask.TimeSpentMinutes += minutes;
                await _taskRepository.UpdateTaskAsync(SelectedTask);
                TimeToAdd = string.Empty;
                OnPropertyChanged(nameof(SelectedTask));
            }
        }
    }
}