using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TodoList.Common.Abstractions.Repositories;
using TodoList.Common.Services;
using TodoList.Data.TodoListInMemory;

namespace TodoList.Services.TodoListService.Startup
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddWalletGateway(this IServiceCollection services)
        {

            services.TryAddScoped<ITodoListRepository, TodoListRepository>();
            services.TryAddScoped<ITodoListApiService, TodoListApiService>();

            services.AddDbContextFactory<TodoContext>(options =>
            {
                options.UseInMemoryDatabase("TodoItemsDB");
            });

            return services;
        }
    }
}
