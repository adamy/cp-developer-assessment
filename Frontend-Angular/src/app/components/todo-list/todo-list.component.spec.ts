import { ComponentFixture, TestBed, fakeAsync, tick  } from '@angular/core/testing';
import { TodoListComponent } from './todo-list.component';
import { TodoService } from 'src/app/services/todo.service';
import { ToastrService } from 'ngx-toastr';
import { MockToastrService } from 'src/app/services/mocks/MockToastrService';
import { of } from 'rxjs';
import { TodoItem } from 'src/app/interfaces/todo-item';

describe('TodoListComponent', () => {
  let component: TodoListComponent;
  let fixture: ComponentFixture<TodoListComponent>;
  let todoService: TodoService;
  let toastrService: ToastrService;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [TodoListComponent],
      providers: [TodoService, { provide: ToastrService, useClass: MockToastrService }]
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(TodoListComponent);
    component = fixture.componentInstance;
    todoService = TestBed.inject(TodoService);
    toastrService = TestBed.inject(ToastrService);
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
  
  it('should call getTodoItems method after called getItems', fakeAsync(() => {
    spyOn(todoService, 'getTodoItems').and.returnValue(Promise.resolve({ isSuccess: true, 
      result: [
        { id: "1233", isCompleted: false, description: "Test item"},
        { id: "1234", isCompleted: true, description: "Test item 2"}
      ], errorMessage: '' }));
    component.getItems();
    expect(todoService.getTodoItems).toHaveBeenCalled();
    fixture.whenStable().then(() => {
      expect(component.items.length).toBe(2);
      expect(component.items[0].id).toBe("1233");
      expect(component.items[0].isCompleted).toBe(false);
      expect(component.items[0].description).toBe("Test item");
      expect(component.items[1].id).toBe("1234");
      expect(component.items[1].isCompleted).toBe(true);
      expect(component.items[1].description).toBe("Test item 2");
    });
  }));

  it('should update item as completed and call getItems method', fakeAsync(() => {
    const item : TodoItem = { id: "1233", isCompleted: false, description: "Test item"};
    spyOn(todoService, 'updateTodoItem').and.returnValue(Promise.resolve({ isSuccess: true, result: 0, errorMessage: '' }));
    spyOn(todoService, 'getTodoItems');
    component.handleMarkAsComplete(item);
    expect(item.isCompleted).toBe(true);
    expect(todoService.updateTodoItem).toHaveBeenCalledWith(item.id, item);
    fixture.whenStable().then(() => {
      expect(component.todoService.getTodoItems).toHaveBeenCalled();
    });
  }));

  it('should update item failed and will not getItems method', fakeAsync(() => {
    const item : TodoItem = { id: "1233", isCompleted: false, description: "Test item"};
    spyOn(todoService, 'updateTodoItem').and.returnValue(Promise.resolve({ isSuccess: true, result: 1, errorMessage: 'error' }));
    spyOn(todoService, 'getTodoItems');
    component.handleMarkAsComplete(item);
    expect(item.isCompleted).toBe(true);
    expect(todoService.updateTodoItem).toHaveBeenCalledWith(item.id, item);
    fixture.whenStable().then(() => {
      expect(component.todoService.getTodoItems).not.toHaveBeenCalled();
    });
  }));
});