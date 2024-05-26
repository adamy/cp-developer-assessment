using TodoList.Common.Models;

namespace TodoList.Common.Abstractions.Repositories
{
    public interface ITodoListRepository
    {
        Task<bool> AddTodoItem(TodoItemViewModel todoItem);
        Task<TodoItemViewModel> GetTodoItem(Guid id);
        Task<ICollection<TodoItemViewModel>> GetTodoItems(bool includeAll = false);
        Task UpdateTodoItem(TodoItemViewModel todoItem);
        Task<bool> TodoItemDescriptionExists(string description);
    }
}