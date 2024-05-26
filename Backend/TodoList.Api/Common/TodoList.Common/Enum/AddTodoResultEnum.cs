using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoList.Common.Enum
{
    public enum AddTodoResultEnum
    {
        Success,
        Duplicate,
        DescriptionIsRequired,
        Error
    }
}
