using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoList.Common.Models;
using TodoList.Data.TodoListInMemory;
using TodoList.Data.TodoListInMemory.DataModels;
using Xunit;

namespace TodoList.Data.TodoListInMemory.Tests
{
    public class TodoListRepositoryTests
    {
        private readonly TodoListRepository _repository;
        private readonly TodoContext _contextForArrange;
        private readonly ILogger<TodoListRepository> _logger;
        private string _databaseName;

        public TodoListRepositoryTests()
        {
            _databaseName = "TestDatabase" + Guid.NewGuid();
            _contextForArrange = new TodoContext(new DbContextOptionsBuilder<TodoContext>()
                .UseInMemoryDatabase(databaseName: _databaseName)
                .Options);

            _logger = Substitute.For<ILogger<TodoListRepository>>();
            _repository = new TodoListRepository(_logger, new TestDbContextFactory(new TodoContext(new DbContextOptionsBuilder<TodoContext>()
                .UseInMemoryDatabase(databaseName: _databaseName)
                .Options)));
        }


        [Fact]
        public async Task GetTodoItems_IncludeAllFalse_ReturnsIncompleteTodoItems()
        {
            // Arrange
            var todoItems = new List<TodoItem>
            {
                new TodoItem { Id = Guid.NewGuid(), Description = "Task 1", IsCompleted = false, CreatedAt = DateTimeOffset.Now },
                new TodoItem { Id = Guid.NewGuid(), Description = "Task 2", IsCompleted = true, CreatedAt = DateTimeOffset.Now },
                new TodoItem { Id = Guid.NewGuid(), Description = "Task 3", IsCompleted = false, CreatedAt = DateTimeOffset.Now }
            };

            await _contextForArrange.TodoItems.AddRangeAsync(todoItems);
            await _contextForArrange.SaveChangesAsync();

            // Act
            var result = await _repository.GetTodoItems(includeAll: false);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.All(result, item => Assert.False(item.IsCompleted));
        }

        [Fact]
        public async Task GetTodoItems_IncludeAllTrue_ReturnsAllTodoItems()
        {
            // Arrange
            var todoItems = new List<TodoItem>
            {
                new TodoItem { Id = Guid.NewGuid(), Description = "Task 1", IsCompleted = false, CreatedAt = DateTimeOffset.Now },
                new TodoItem { Id = Guid.NewGuid(), Description = "Task 2", IsCompleted = true, CreatedAt = DateTimeOffset.Now },
                new TodoItem { Id = Guid.NewGuid(), Description = "Task 3", IsCompleted = false, CreatedAt = DateTimeOffset.Now }
            };

            await _contextForArrange.TodoItems.AddRangeAsync(todoItems);
            await _contextForArrange.SaveChangesAsync();

            // Act
            var result = await _repository.GetTodoItems(includeAll: true);

            // Assert
            Assert.Equal(3, result.Count);
        }

        [Fact]
        public async Task GetTodoItem_ExistingId_ReturnsTodoItem()
        {
            // Arrange
            var todoItem = new TodoItem { Id = Guid.NewGuid(), Description = "Task 1", IsCompleted = false, CreatedAt = DateTimeOffset.Now };

            await _contextForArrange.TodoItems.AddAsync(todoItem);
            await _contextForArrange.SaveChangesAsync();

            // Act
            var result = await _repository.GetTodoItem(todoItem.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(todoItem.Id, result.Id);
            Assert.Equal(todoItem.Description, result.Description);
            Assert.Equal(todoItem.IsCompleted, result.IsCompleted);
        }

        [Fact]
        public async Task GetTodoItem_NonExistingId_ReturnsNull()
        {
            // Arrange
            var nonExistingId = Guid.NewGuid();

            // Act
            var result = await _repository.GetTodoItem(nonExistingId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateTodoItem_ExistingTodoItem_UpdatesTodoItem()
        {
            // Arrange
            var todoItem = new TodoItem { Id = Guid.NewGuid(), Description = "Task 1", IsCompleted = false, CreatedAt = DateTimeOffset.Now };

            await _contextForArrange.TodoItems.AddAsync(todoItem);
            await _contextForArrange.SaveChangesAsync();

            var updatedTodoItem = new TodoItemViewModel
            {
                Id = todoItem.Id,
                Description = "Updated Task",
                IsCompleted = true
            };

            // Act
            await _repository.UpdateTodoItem(updatedTodoItem);

            // Assert
            await _contextForArrange.Entry(todoItem).ReloadAsync();
            var result = await _contextForArrange.TodoItems.FindAsync(todoItem.Id);
            Assert.Equal(updatedTodoItem.Description, result.Description);
            Assert.Equal(updatedTodoItem.IsCompleted, result.IsCompleted);
        }

        [Fact]
        public async Task UpdateTodoItem_NonExistingTodoItem_DoesNotUpdateTodoItem()
        {
            // Arrange
            var nonExistingId = Guid.NewGuid();
            var updatedTodoItem = new TodoItemViewModel
            {
                Id = nonExistingId,
                Description = "Updated Task",
                IsCompleted = true
            };

            // Act
            await _repository.UpdateTodoItem(updatedTodoItem);

            // Assert
            var result = await _contextForArrange.TodoItems.FindAsync(nonExistingId);
            Assert.Null(result);
        }

        [Fact]
        public async Task AddTodoItem_ValidTodoItem_ReturnsTrue()
        {
            // Arrange
            var todoItem = new TodoItemViewModel
            {
                Id = Guid.NewGuid(),
                Description = "New Task",
                IsCompleted = false
            };

            // Act
            var result = await _repository.AddTodoItem(todoItem);

            // Assert
            Assert.True(result);
            var addedItem = await _contextForArrange.TodoItems.FindAsync(todoItem.Id);
            Assert.NotNull(addedItem);
            Assert.Equal(todoItem.Description, addedItem.Description);
            Assert.Equal(todoItem.IsCompleted, addedItem.IsCompleted);
        }

        [Fact]
        public async Task TodoItemDescriptionExists_ExistingDescription_ReturnsTrue()
        {
            // Arrange
            var description = "Existing Task";
            var todoItem = new TodoItem { Id = Guid.NewGuid(), Description = description, IsCompleted = false, CreatedAt = DateTimeOffset.Now };

            await _contextForArrange.TodoItems.AddAsync(todoItem);
            await _contextForArrange.SaveChangesAsync();

            // Act
            var result = await _repository.TodoItemDescriptionExists(description);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task TodoItemDescriptionExists_NonExistingDescription_ReturnsFalse()
        {
            // Arrange
            var nonExistingDescription = "Non-existing Task";

            // Act
            var result = await _repository.TodoItemDescriptionExists(nonExistingDescription);

            // Assert
            Assert.False(result);
        }
    }

    public class TestDbContextFactory : IDbContextFactory<TodoContext>
    {
        private readonly TodoContext _context;

        public TestDbContextFactory(TodoContext context)
        {
            _context = context;
        }

        public TodoContext CreateDbContext()
        {
            return _context;
        }
    }
}
