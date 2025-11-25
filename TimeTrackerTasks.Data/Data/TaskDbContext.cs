using Microsoft.EntityFrameworkCore;
using TimeTrackerTasks.Core.Models;

namespace TimeTrackerTasks.Data.Data
{
    public class TaskDbContext : DbContext
    {
        public DbSet<TaskItem> Tasks => Set<TaskItem>();  // ← ИЗМЕНИТЬ

        // ДОБАВИТЬ конструктор
        public TaskDbContext(DbContextOptions<TaskDbContext> options) : base(options) { }

        // ИЛИ оставить пустой конструктор для миграций
        public TaskDbContext() { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=TimeTrackerTasks.db");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TaskItem>(entity =>  // ← ИЗМЕНИТЬ
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title)
                      .IsRequired()
                      .HasMaxLength(100);
                entity.Property(e => e.Description)
                      .HasMaxLength(500);
                entity.Property(e => e.CreatedDate)
                      .IsRequired();
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}