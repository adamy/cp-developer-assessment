import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { AppComponent } from './app.component';
import { TodoListComponent } from './components/todo-list/todo-list.component';
import { TodoService } from './services/todo.service';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { MockToastrService } from './services/mocks/MockToastrService';

describe('AppComponent', () => {
  let component: AppComponent;
  let fixture: ComponentFixture<AppComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [AppComponent, TodoListComponent],
      providers: [TodoService, FormBuilder,  { provide: ToastrService, useClass: MockToastrService }],
      imports: [ReactiveFormsModule],
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AppComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create the app', () => {
    expect(component).toBeTruthy();
  });

  it('should call getItems method on ngAfterViewInit', () => {
    spyOn(component.todoListComponent, 'getItems');
    component.ngAfterViewInit();
    expect(component.todoListComponent.getItems).toHaveBeenCalled();
  });

  it('should reset form and call getItems method on successful form submission', fakeAsync(() => {
    spyOn(component.todoListComponent, 'getItems');
    spyOn(component.descForm, 'reset');
    spyOn(component.todoService, 'addTodoItem').and.returnValue(Promise.resolve({ isSuccess: true, result: 0, errorMessage: ''}));    
    spyOnProperty(component.descForm, 'valid').and.returnValue(true);
    component.onSubmit(component.descForm);
    expect(component.isPosting).toBeTrue();
    expect(component.todoService.addTodoItem).toHaveBeenCalled();
    fixture.whenStable().then(() => {
        expect(component.todoListComponent.getItems).toHaveBeenCalled();
        expect(component.descForm.reset).toHaveBeenCalled(); 
    });
  }));

  it('should not reset form and call getItems method on validation error', () => {
    spyOn(component.todoListComponent, 'getItems');
    spyOn(component.todoService, 'addTodoItem').and.returnValue(Promise.resolve({ isSuccess: true, result: 0, errorMessage: ''}));    
    spyOnProperty(component.descForm, 'valid').and.returnValue(false);
    component.onSubmit(component.descForm);
    expect(component.isPosting).toBeFalse();
  });

  it('should reset form and call getItems method on failed form submission', fakeAsync(() => {
    spyOn(component.todoListComponent, 'getItems');
    spyOn(component.descForm, 'reset');
    spyOn(component.todoService, 'addTodoItem').and.returnValue(Promise.resolve({ isSuccess: true, result: 1, errorMessage: 'Test error'}));    
    spyOnProperty(component.descForm, 'valid').and.returnValue(true);
    component.onSubmit(component.descForm);
    expect(component.isPosting).toBeTrue();
    expect(component.todoService.addTodoItem).toHaveBeenCalled();
    fixture.whenStable().then(() => {
        expect(component.todoListComponent.getItems).not.toHaveBeenCalled();
        expect(component.descForm.reset).not.toHaveBeenCalled(); 
    });
  }));
});

