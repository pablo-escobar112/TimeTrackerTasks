using System.Windows;
using TimeTrackerTasks.WPF.ViewModels;

namespace TimeTrackerTasks.WPF
{
    public partial class MainWindow : Window
    {
        public MainWindow(MainViewModel viewModel)
        {
            InitializeComponent();

            // Добавим проверку на null
            if (viewModel != null)
            {
                DataContext = viewModel;
            }
        }
    }

}