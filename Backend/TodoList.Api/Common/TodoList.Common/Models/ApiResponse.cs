using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoList.Common.Models
{
    public class ApiResponse
    {
        public bool IsSuccess { get; set; }

        public string ErrorMessage { get; set; }
    }

    public class ApiResponse<T> : ApiResponse
    {
        public T Result { get; set; }                
    }    
}
