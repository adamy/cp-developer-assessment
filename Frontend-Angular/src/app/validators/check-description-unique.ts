
import { TodoService } from '../services/todo.service';
import { ApiResponseWithType } from '../interfaces/api-response-with-type';
import { AsyncValidatorFn, AbstractControl} from '@angular/forms';
import { of, delay, switchMap} from 'rxjs';
import { ToastrService } from 'ngx-toastr';


export function checkDescriptionUnique(
    todoService: TodoService, toastr: ToastrService
  ): AsyncValidatorFn {

    return (control: AbstractControl) => {
        return of(control.value).pipe(
            delay(500),
            switchMap(() => 
                todoService.todoItemDescriptionExists(control.value)
                    .then((resp: ApiResponseWithType<boolean>) => {
                    console.log("test", resp);
                    if(resp.isSuccess){       
                        if(resp.result){
                            return {
                                isDescriptionUnique: true
                            };
                        } 
                        return null;
                        
                    }
                    return null;})
                    .catch((error: any) => {
                        toastr.error(`${error}`, "Service error");
                        return null;
                      })
            ));                          
    };
  }