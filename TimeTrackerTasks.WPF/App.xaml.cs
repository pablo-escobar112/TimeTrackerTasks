using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using TimeTrackerTasks.Core.Interfaces;
using TimeTrackerTasks.Data.Data;
using TimeTrackerTasks.Data.Repositories;
using TimeTrackerTasks.WPF.ViewModels;

namespace TimeTrackerTasks.WPF
{
    public partial class App : Application
    {
        private ServiceProvider _serviceProvider;

        public App()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();
        }

        private void ConfigureServices(ServiceCollection services)
        {
            services.AddDbContext<TaskDbContext>(options =>
            {
                options.UseSqlite("Data Source=TimeTrackerTasks.db");
            });

            services.AddScoped<ITaskRepository, TaskRepository>();
            services.AddTransient<MainViewModel>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Создаем БД если ее нет
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<TaskDbContext>();
                context.Database.EnsureCreated();
            }

            // Создаем MainViewModel и MainWindow
            var viewModel = _serviceProvider.GetService<MainViewModel>();
            var mainWindow = new MainWindow(viewModel);
            mainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _serviceProvider?.Dispose();
            base.OnExit(e);
        }
    }
}