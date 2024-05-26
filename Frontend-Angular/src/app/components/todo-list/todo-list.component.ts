import { Component, OnInit, inject } from '@angular/core';
import { TodoItem } from 'src/app/interfaces/todo-item';
import { TodoService } from 'src/app/services/todo.service';
import { ApiResponseWithType } from 'src/app/interfaces/api-response-with-type';

@Component({
  selector: 'app-todo-list',
  templateUrl: './todo-list.component.html',
  styleUrls: ['./todo-list.component.css']
})
export class TodoListComponent{
  items: TodoItem[] = [];
  todoService: TodoService = inject(TodoService);

  getItems() {
    this.todoService.getTodoItems().then((resp : ApiResponseWithType<TodoItem[]>) => {
      if(resp.isSuccess){
        this.items = resp.result;
      }
    });
  }
  
  handleMarkAsComplete(item: TodoItem) {
    item.isCompleted = true;
    this.todoService.updateTodoItem(item.id, item)
      .then((resp: ApiResponseWithType<Number>) => {
        if(resp.isSuccess){
          this.getItems();
        }
      });
  }
}
