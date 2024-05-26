
import { TodoService } from '../services/todo.service';
import { ApiResponseWithType } from '../interfaces/api-response-with-type';
import { AsyncValidatorFn, AbstractControl} from '@angular/forms';
import { of, delay, switchMap} from 'rxjs';


export function checkDescriptionUnique(
    todoService: TodoService,
  ): AsyncValidatorFn {
    return (control: AbstractControl) => {
        return of(control.value).pipe(
            delay(500),
            switchMap(() => 
                todoService.todoItemDescriptionExists(control.value)
                    .then((resp: ApiResponseWithType<boolean>) => {
                    console.log("test", resp);
                    if(resp.isSuccess){        
                        return {
                        isDescriptionUnique: resp.result
                        }
                    }
                    return null;})
            ));        
    };
  }