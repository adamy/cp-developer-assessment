using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoList.Data.TodoListInMemory.DataModels
{
    public class TodoItem
    {
        public Guid Id { get; set; }

        public string Description { get; set; }

        public bool IsCompleted { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
    }
}
