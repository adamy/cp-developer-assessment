import { Component, OnInit, inject } from '@angular/core';
import { TodoItem } from 'src/app/interfaces/todo-item';
import { TodoService } from 'src/app/services/todo.service';
import { ApiResponseWithType } from 'src/app/interfaces/api-response-with-type';
import { ToastrService } from 'ngx-toastr';


@Component({
  selector: 'app-todo-list',
  templateUrl: './todo-list.component.html',
  styleUrls: ['./todo-list.component.css']
})
export class TodoListComponent{
  items: TodoItem[] = [];
  todoService: TodoService = inject(TodoService);

  public constructor(private toastr: ToastrService) {
  }

  getItems() {
    this.todoService.getTodoItems().then((resp : ApiResponseWithType<TodoItem[]>) => {
      if(resp.isSuccess){
        this.items = resp.result;
      }else{
        this.toastr.error("Failed to load todo items", "Error");
      }
    }).catch((error: any) => {
      this.toastr.error(`${error}`, "Service error");
    });
  }
  
  handleMarkAsComplete(item: TodoItem) {
    item.isCompleted = true;
    this.todoService.updateTodoItem(item.id, item)
      .then((resp: ApiResponseWithType<Number>) => {
        if(resp.isSuccess){
          this.getItems();
          this.toastr.success("Todo item marked as complete");
        }else{
          this.toastr.error("Mark as complete failed", "Error");
        }
      })
      .catch((error: any) => {
        this.toastr.error(`${error}`, "Service error");
      });
  }
}
