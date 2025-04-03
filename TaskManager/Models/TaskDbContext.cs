
// In Data/TaskDbContext.cs
using Microsoft.EntityFrameworkCore;
using TaskManager.Models;

public class TaskDbContext : DbContext
{
    public TaskDbContext(DbContextOptions<TaskDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<ToDo> ToDos { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Explicitly configure the table name and schema
        modelBuilder.Entity<User>().ToTable("Users");

        // Add any additional configurations here
    }
}