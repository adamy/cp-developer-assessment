using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoList.Common.Enum;
using TodoList.Common.Models;
using TodoList.Common.Services;

namespace TodoList.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoItemsController : ControllerBase
    {
        public TodoItemsController(ILogger<TodoItemsController> logger, ITodoListApiService todoListApiService)
        {
            _todoListApiService = todoListApiService;
            _logger = logger;
        }

        private readonly ILogger<TodoItemsController> _logger;
        private readonly ITodoListApiService _todoListApiService;

        // GET: api/TodoItems
        [HttpGet]
        public async Task<ApiResponse<ICollection<TodoItemViewModel>>> GetTodoItems()
        {
            return await _todoListApiService.GetTodoItems();
        }

        // GET: api/TodoItems/...
        [HttpGet("{id}")]
        public async Task<ApiResponse<TodoItemViewModel>> GetTodoItem(Guid id)
        {
            return await _todoListApiService.GetTodoItem(id);
        }

        // PUT: api/TodoItems/... 
        [HttpPut("{id}")]
        public async Task<ApiResponse<UpdateTodoResultEnum>> UpdateTodoItem(Guid id, TodoItemViewModel todoItem)
        {
            if (id != todoItem.Id)
            {
                return new ApiResponse<UpdateTodoResultEnum>
                {
                    Result = UpdateTodoResultEnum.Error,
                    IsSuccess = false
                };
            }

            return await _todoListApiService.UpdateTodoItem(todoItem);
        } 

        // POST: api/TodoItems 
        [HttpPost]
        public async Task<ApiResponse<AddTodoResultEnum>> AddTodoItem(TodoItemAddViewModel todoItem)
        {
            return await _todoListApiService.AddTodoItem(todoItem);
        }

        [HttpPost("exist")]
        public async Task<ApiResponse<bool>> TodoItemDescriptionExists(string description)
        {
            return await _todoListApiService.TodoItemDescriptionExists(description);
        }
    }
}
