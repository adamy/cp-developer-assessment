using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using TodoList.Common.Abstractions.Repositories;
using TodoList.Common.Enum;
using TodoList.Common.Models;
using TodoList.Services.TodoListService;
using Xunit;

namespace TodoList.Services.TodoListService.Tests
{
    public class TodoListApiServiceTests
    {
        private readonly ITodoListRepository _todoListRepository;
        private readonly ILogger<TodoListApiService> _logger;
        private readonly TodoListApiService _todoListApiService;

        public TodoListApiServiceTests()
        {
            _todoListRepository = Substitute.For<ITodoListRepository>();
            _logger = Substitute.For<ILogger<TodoListApiService>>();
            _todoListApiService = new TodoListApiService(_logger, _todoListRepository);
        }

        [Fact]
        public async Task GetTodoItems_ShouldReturnApiResponseWithTodoItems()
        {
            // Arrange
            var todoItems = new List<TodoItemViewModel>
            {
                new TodoItemViewModel
                {
                    Id = Guid.NewGuid(),
                    Description = "Todo 1",
                    IsCompleted = false
                },
                new TodoItemViewModel
                {
                    Id = Guid.NewGuid(),
                    Description = "Todo 2",
                    IsCompleted = true
                }
            };
            _todoListRepository.GetTodoItems().Returns(todoItems);

            // Act
            var result = await _todoListApiService.GetTodoItems();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(todoItems, result.Result);
        }

        [Fact]
        public async Task GetTodoItem_WithValidId_ShouldReturnApiResponseWithTodoItem()
        {
            // Arrange
            var id = Guid.NewGuid();
            var todoItem = new TodoItemViewModel
            {
                Id = id,
                Description = "Todo 1",
                IsCompleted = false
            };
            _todoListRepository.GetTodoItem(id).Returns(todoItem);

            // Act
            var result = await _todoListApiService.GetTodoItem(id);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Result);
            Assert.Equal(todoItem.Description, result.Result.Description);
            Assert.Equal(todoItem.Id, result.Result.Id);
            Assert.Equal(todoItem.IsCompleted, result.Result.IsCompleted);
        }

        [Fact]
        public async Task GetTodoItem_WithInvalidId_ShouldReturnApiResponseWithNullTodoItem()
        {
            // Arrange
            _todoListRepository.GetTodoItem(Arg.Any<Guid>()).Returns((TodoItemViewModel)null);

            // Act
            var result = await _todoListApiService.GetTodoItem(Guid.NewGuid());

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Null(result.Result);
        }

        [Fact]
        public async Task AddTodoItem_WithValidTodoItem_ShouldReturnApiResponseWithSuccessResult()
        {
            // Arrange
            var todoItem = new TodoItemAddViewModel
            {
                Description = "New Todo",
            };
            _todoListRepository.TodoItemDescriptionExists(todoItem.Description).Returns(false);
            _todoListRepository.AddTodoItem(Arg.Any<TodoItemViewModel>()).Returns(true);

            // Act
            var result = await _todoListApiService.AddTodoItem(todoItem);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(AddTodoResultEnum.Success, result.Result);
        }

        [Fact]
        public async Task AddTodoItem_WithNullTodoItem_ShouldReturnApiResponseWithErrorResult()
        {
            // Arrange
            TodoItemAddViewModel todoItem = null;

            // Act
            var result = await _todoListApiService.AddTodoItem(todoItem);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(AddTodoResultEnum.Error, result.Result);
            Assert.Equal("Todo item is null", result.ErrorMessage);
        }

        [Fact]
        public async Task AddTodoItem_WithEmptyDescription_ShouldReturnApiResponseWithDescriptionIsRequiredResult()
        {
            // Arrange
            var todoItem = new TodoItemAddViewModel
            {
                Description = string.Empty,
            };

            // Act
            var result = await _todoListApiService.AddTodoItem(todoItem);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(AddTodoResultEnum.DescriptionIsRequired, result.Result);
            Assert.Equal("Description is required", result.ErrorMessage);
        }

        [Fact]
        public async Task AddTodoItem_WithExistingDescription_ShouldReturnApiResponseWithDuplicateResult()
        {
            // Arrange
            var todoItem = new TodoItemAddViewModel
            {
                Description = "Existing Todo",
            };
            _todoListRepository.TodoItemDescriptionExists(todoItem.Description).Returns(true);

            // Act
            var result = await _todoListApiService.AddTodoItem(todoItem);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(AddTodoResultEnum.Duplicate, result.Result);
            Assert.Equal("Description already exists", result.ErrorMessage);
        }

        [Fact]
        public async Task AddTodoItem_WithRepositoryError_ShouldReturnApiResponseWithErrorResult()
        {
            // Arrange
            var todoItem = new TodoItemAddViewModel
            {
                Description = "New Todo",
            };
            _todoListRepository.TodoItemDescriptionExists(todoItem.Description).Returns(false);
            _todoListRepository.AddTodoItem(Arg.Any<TodoItemViewModel>()).Returns(false);

            // Act
            var result = await _todoListApiService.AddTodoItem(todoItem);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(AddTodoResultEnum.Error, result.Result);
            Assert.Equal("Error adding todo item", result.ErrorMessage);
        }

        [Fact]
        public async Task UpdateTodoItem_WithExistingTodoItem_ShouldReturnApiResponseWithSuccessResult()
        {
            // Arrange
            var todoItem = new TodoItemViewModel
            {
                Id = Guid.NewGuid(),
                Description = "Todo 1",
                IsCompleted = false
            };
            _todoListRepository.GetTodoItem(todoItem.Id).Returns(todoItem);

            // Act
            var result = await _todoListApiService.UpdateTodoItem(todoItem);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(UpdateTodoResultEnum.Success, result.Result);
        }

        [Fact]
        public async Task UpdateTodoItem_WithNonExistingTodoItem_ShouldReturnApiResponseWithNotFoundResult()
        {
            // Arrange
            var todoItem = new TodoItemViewModel
            {
                Id = Guid.NewGuid(),
                Description = "Todo 1",
                IsCompleted = false
            };
            _todoListRepository.GetTodoItem(todoItem.Id).Returns((TodoItemViewModel)null);

            // Act
            var result = await _todoListApiService.UpdateTodoItem(todoItem);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(UpdateTodoResultEnum.NotFound, result.Result);
            Assert.Equal("Todo item not found", result.ErrorMessage);
        }

        [Fact]
        public async Task UpdateTodoItem_WithRepositoryError_ShouldReturnApiResponseWithErrorResult()
        {
            // Arrange
            var todoItem = new TodoItemViewModel
            {
                Id = Guid.NewGuid(),
                Description = "Todo 1",
                IsCompleted = false
            };
            _todoListRepository.GetTodoItem(todoItem.Id).Throws(new Exception("Repository error"));

            // Act
            var result = await _todoListApiService.UpdateTodoItem(todoItem);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(UpdateTodoResultEnum.Error, result.Result);
            Assert.Equal("Error updating todo item", result.ErrorMessage);
        }

        [Fact]
        public async Task TodoItemDescriptionExists_WithExistingDescription_ShouldReturnApiResponseWithTrueResult()
        {
            // Arrange
            var description = "Existing Todo";
            _todoListRepository.TodoItemDescriptionExists(description).Returns(true);

            // Act
            var result = await _todoListApiService.TodoItemDescriptionExists(description);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(result.Result);
        }

        [Fact]
        public async Task TodoItemDescriptionExists_WithNonExistingDescription_ShouldReturnApiResponseWithFalseResult()
        {
            // Arrange
            var description = "New Todo";
            _todoListRepository.TodoItemDescriptionExists(description).Returns(false);

            // Act
            var result = await _todoListApiService.TodoItemDescriptionExists(description);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.False(result.Result);
        }

        [Fact]
        public async Task TodoItemDescriptionExists_WithRepositoryError_ShouldReturnApiResponseWithErrorResult()
        {
            // Arrange
            var description = "Existing Todo";
            _todoListRepository.TodoItemDescriptionExists(description).Throws(new Exception("Repository error"));

            // Act
            var result = await _todoListApiService.TodoItemDescriptionExists(description);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Error checking if description exists", result.ErrorMessage);
        }
    }
}
