using Microsoft.EntityFrameworkCore;
using TodoList.Data.TodoListInMemory.DataModels;

namespace TodoList.Data.TodoListInMemory
{
    public class TodoContext : DbContext
    {
        public TodoContext(DbContextOptions<TodoContext> options)
            : base(options)
        {
        }

        public DbSet<TodoItem> TodoItems { get; set; }
    }
}
