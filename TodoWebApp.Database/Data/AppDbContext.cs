using Microsoft.EntityFrameworkCore;
using TodoWebApp.Models.Entities;

namespace TodoWebApp.Database.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }

    public DbSet<TaskModel> Tasks { get; set; }
}