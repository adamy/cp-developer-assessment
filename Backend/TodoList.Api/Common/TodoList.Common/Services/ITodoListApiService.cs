using TodoList.Common.Enum;
using TodoList.Common.Models;

namespace TodoList.Common.Services
{
    public interface ITodoListApiService
    {
        Task<ApiResponse<AddTodoResultEnum>> AddTodoItem(TodoItemAddViewModel todoItem);
        Task<ApiResponse<TodoItemViewModel>> GetTodoItem(Guid id);
        Task<ApiResponse<ICollection<TodoItemViewModel>>> GetTodoItems();
        Task<ApiResponse<UpdateTodoResultEnum>> UpdateTodoItem(TodoItemViewModel todoItem);
        Task<ApiResponse<bool>> TodoItemDescriptionExists(string description);
    }
}