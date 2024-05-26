using Azure;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoList.Common.Abstractions.Repositories;
using TodoList.Common.Enum;
using TodoList.Common.Models;
using TodoList.Common.Services;
using TodoList.Data.TodoListInMemory.DataModels;

namespace TodoList.Services.TodoListService
{
    public class TodoListApiService : ITodoListApiService
    {
        public TodoListApiService(ILogger<TodoListApiService> logger, ITodoListRepository todoListRepository)
        {
            _todoListRepository = todoListRepository;
            this.logger = logger;
        }

        ITodoListRepository _todoListRepository;
        ILogger<TodoListApiService> logger;

        public async Task<ApiResponse<ICollection<TodoItemViewModel>>> GetTodoItems()
        {
            try
            {
                var todoItems = await _todoListRepository.GetTodoItems();
                return new ApiResponse<ICollection<TodoItemViewModel>>
                {
                    Result = todoItems,
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                var errorMessage = "Error getting todo items";
                logger.LogError(ex, errorMessage);
                return new ApiResponse<ICollection<TodoItemViewModel>>
                {
                    Result = null,
                    ErrorMessage = errorMessage,
                    IsSuccess = false
                };
            }
        }

        public async Task<ApiResponse<TodoItemViewModel>> GetTodoItem(Guid id)
        {
            try
            {
                var todoItem = await _todoListRepository.GetTodoItem(id);
                if (todoItem == null)
                {
                    return new ApiResponse<TodoItemViewModel>
                    {
                        Result = null,
                        IsSuccess = false
                    };
                }

                return new ApiResponse<TodoItemViewModel>
                {
                    Result = new TodoItemViewModel
                    {
                        Description = todoItem.Description, 
                        Id = todoItem.Id,
                        IsCompleted = todoItem.IsCompleted
                    },
                    IsSuccess = false
                };
            }
            catch (Exception ex)
            {
                var errorMessage = "Error getting todo item";
                logger.LogError(ex, errorMessage);
                return new ApiResponse<TodoItemViewModel>
                {
                    Result = null,
                    ErrorMessage = errorMessage,
                    IsSuccess = false
                };
            }
        }

        public async Task<ApiResponse<AddTodoResultEnum>> AddTodoItem(TodoItemAddViewModel todoItem)
        {
            if (todoItem == null)
            {
                return new ApiResponse<AddTodoResultEnum>
                {
                    IsSuccess = false,
                    ErrorMessage = "Todo item is null",
                    Result = AddTodoResultEnum.Error
                };
            }

            if(string.IsNullOrWhiteSpace(todoItem.Description))
            {
                return new ApiResponse<AddTodoResultEnum>
                {
                    IsSuccess = false,
                    ErrorMessage = "Description is required",
                    Result = AddTodoResultEnum.DescriptionIsRequired
                };
            }

            try
            {
                var descExists = await _todoListRepository.TodoItemDescriptionExists(todoItem.Description);
                if (descExists)
                {
                    return new ApiResponse<AddTodoResultEnum>
                    {
                        IsSuccess = false,
                        ErrorMessage = "Description already exists",
                        Result = AddTodoResultEnum.Duplicate
                    };
                }
                var result = await _todoListRepository.AddTodoItem(new TodoItemViewModel
                {
                    Id = Guid.NewGuid(),
                    Description = todoItem.Description,
                    IsCompleted = false
                });
                if (result)
                {
                    return new ApiResponse<AddTodoResultEnum>
                    {
                        IsSuccess = true,
                        Result = AddTodoResultEnum.Success
                    };
                }
                else
                {
                    return new ApiResponse<AddTodoResultEnum>
                    {
                        IsSuccess = false,
                        ErrorMessage = "Error adding todo item",
                        Result = AddTodoResultEnum.Error
                    };
                }

            }
            catch (Exception ex)
            {
                var errorMessage = "Error adding ";
                logger.LogError(ex, errorMessage);
                return new ApiResponse<AddTodoResultEnum>
                {
                    IsSuccess = false,
                    ErrorMessage = errorMessage,
                    Result = AddTodoResultEnum.Error
                };
            }
        }

        public async Task<ApiResponse<UpdateTodoResultEnum>> UpdateTodoItem(TodoItemViewModel todoItem)
        {
            try
            {
                var existingTodoItem = await _todoListRepository.GetTodoItem(todoItem.Id);
                if (existingTodoItem != null)
                {
                    await _todoListRepository.UpdateTodoItem(todoItem);
                    return new ApiResponse<UpdateTodoResultEnum>
                    {
                        Result = UpdateTodoResultEnum.Success,
                        IsSuccess = true
                    };
                }
                else
                {
                    return new ApiResponse<UpdateTodoResultEnum>
                    {
                        Result = UpdateTodoResultEnum.NotFound,
                        IsSuccess = false,
                        ErrorMessage = "Todo item not found"
                    };
                }
            }
            catch (Exception ex)
            {
                var errorMessage = "Error updating todo item";
                logger.LogError(ex, errorMessage);
                return new ApiResponse<UpdateTodoResultEnum>
                {
                    Result = UpdateTodoResultEnum.Error,
                    ErrorMessage = errorMessage,
                    IsSuccess = false
                };
            }
        }

        public async Task<ApiResponse<bool>> TodoItemDescriptionExists(string description)
        {
            try
            {
                var result = await _todoListRepository.TodoItemDescriptionExists(description);
                return new ApiResponse<bool>
                {
                    Result = result,
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                var errorMessage = "Error checking if description exists";
                logger.LogError(ex, errorMessage);
                return new ApiResponse<bool>
                {
                    Result = false,
                    ErrorMessage = errorMessage,
                    IsSuccess = false
                };
            }
        }
    }
}
