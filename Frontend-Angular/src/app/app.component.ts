import { Component, ViewChild, AfterViewInit, OnInit, inject } from '@angular/core';
import { TodoListComponent } from './components/todo-list/todo-list.component';
import { TodoService } from './services/todo.service';
import { TodoItemAdd } from './interfaces/todo-item-add';
import { ApiResponseWithType } from './interfaces/api-response-with-type';
import { FormGroup,  FormBuilder,  Validators} from '@angular/forms';
import { checkDescriptionUnique } from './validators/check-description-unique';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements AfterViewInit, OnInit{
  todoService: TodoService = inject(TodoService);
  descriptionInputError: string = '';
  descForm: FormGroup ;
  isPosting: boolean = false;

  @ViewChild(TodoListComponent) todoListComponent;

  public constructor(private formBuilder: FormBuilder, private toastr: ToastrService) {
  }

  ngOnInit(){
    this.descForm = this.formBuilder.group({
      description: ['', null, [checkDescriptionUnique(this.todoService, this.toastr)]]
    });
  }

  ngAfterViewInit(){
    this.todoListComponent.getItems();
  }

  onSubmit(form: FormGroup){    
    if(!form.valid){
      return;
    }
    this.isPosting = true;
    this.todoService.addTodoItem(<TodoItemAdd>{
      description: form.value.description
    })
    .then((resp: ApiResponseWithType<Number>) => {
      if(resp.isSuccess){
        if(resp.result == 0){
          this.todoListComponent.getItems();
          this.descForm.reset();
          this.toastr.success("Todo item added");
        }else{
          this.toastr.error("Todo not added", "Error");
        }
      }
    })
    .catch((error: any) => {
      this.toastr.error(`${error}`, "Service error");
    })
    .finally(() => {
      this.isPosting = false;
    });
  }
}
