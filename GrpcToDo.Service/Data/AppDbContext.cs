using Grpc.Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace GrpcToDo.Service.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<ToDoItem> ToDoItems { get; set; }

    }
}
