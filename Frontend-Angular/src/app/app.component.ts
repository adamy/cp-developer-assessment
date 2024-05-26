import { Component, ViewChild, AfterViewInit, OnInit, inject } from '@angular/core';
import { TodoListComponent } from './components/todo-list/todo-list.component';
import { TodoService } from './services/todo.service';
import { TodoItemAdd } from './interfaces/todo-item-add';
import { ApiResponseWithType } from './interfaces/api-response-with-type';
import { FormGroup,  FormBuilder,  Validators} from '@angular/forms';
import { checkDescriptionUnique } from './validators/check-description-unique';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements AfterViewInit, OnInit{
  todoService: TodoService = inject(TodoService);
  descriptionInputError: string = '';
  descForm: FormGroup ;

  @ViewChild(TodoListComponent) todoListComponent;

  public constructor(private formBuilder: FormBuilder) {
  }

  ngOnInit(){
    this.descForm = this.formBuilder.group({
      description: ['', [Validators.required, Validators.min(1), Validators.max(150)
      ], [checkDescriptionUnique(this.todoService)]]
    });
  }

  ngAfterViewInit(){
    this.todoListComponent.getItems();
  }

  onSubmit(form: FormGroup){
    
    this.todoService.addTodoItem(<TodoItemAdd>{
      description: form.value.description
    })
    .then((resp: ApiResponseWithType<Number>) => {
      if(resp.isSuccess){
        if(resp.result == 0){
          this.todoListComponent.getItems();
          this.descForm.reset();
        }
      }
    });
  }
}
