using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TodoList.Common.Abstractions.Repositories;
using TodoList.Common.Models;
using TodoList.Data.TodoListInMemory.DataModels;

namespace TodoList.Data.TodoListInMemory
{
    public class TodoListRepository : ITodoListRepository
    {
        public TodoListRepository(ILogger<TodoListRepository> logger, IDbContextFactory<TodoContext> contextFactory)
        {
            _contextFactory = contextFactory;
            _logger = logger;
        }

        private readonly IDbContextFactory<TodoContext> _contextFactory;
        ILogger<TodoListRepository> _logger;

        public async Task<ICollection<TodoItemViewModel>> GetTodoItems(bool includeAll = false)
        {
            using var context = _contextFactory.CreateDbContext();

            var query = context.TodoItems.AsQueryable();
            if (!includeAll)
            {
                query = query.Where(i => i.IsCompleted == false);
            }

            return await query.OrderByDescending(x => x.CreatedAt)
                .Select(x => new TodoItemViewModel
                {
                    Id = x.Id,
                    Description = x.Description,
                    IsCompleted = x.IsCompleted
                }).ToListAsync();
        }

        public async Task<TodoItemViewModel> GetTodoItem(Guid id)
        {
            using var context = _contextFactory.CreateDbContext();
            var result = await context.TodoItems.FindAsync(id);

            if (result == null)
            {
                return null;
            }

            return new TodoItemViewModel
            {
                Id = result.Id,
                Description = result.Description,
                IsCompleted = result.IsCompleted
            };
        }

        public async Task UpdateTodoItem(TodoItemViewModel todoItem)
        {
            using var context = _contextFactory.CreateDbContext();
            var entity = context.TodoItems.Find(todoItem.Id);

            if (entity == null)
            {
                return;
            }

            entity.Description = todoItem.Description;
            entity.IsCompleted = todoItem.IsCompleted;

            await context.SaveChangesAsync();
        }

        public async Task<bool> AddTodoItem(TodoItemViewModel todoItem)
        {
            using var context = _contextFactory.CreateDbContext();
            context.TodoItems.Add(new TodoItem
            {
                Id = todoItem.Id,
                Description = todoItem.Description,
                IsCompleted = todoItem.IsCompleted,
                CreatedAt = DateTimeOffset.Now
            });

            return await context.SaveChangesAsync() > 0;
        }
        
        public async Task<bool> TodoItemDescriptionExists(string description)
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.TodoItems.AnyAsync(e => e.Description.ToLowerInvariant() == description.ToLowerInvariant() && !e.IsCompleted);
        }
    }
}
