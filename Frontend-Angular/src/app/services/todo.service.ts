import { Injectable } from '@angular/core';
import { TodoItem } from '../interfaces/todo-item';
import { TodoItemAdd } from '../interfaces/todo-item-add';
import { ApiResponse } from '../interfaces/api-response';
import { ApiResponseWithType } from '../interfaces/api-response-with-type';

@Injectable({
  providedIn: 'root'
})
export class TodoService {

  constructor() { }
  url = 'http://localhost:64463/api/TodoItems';
  headers : Headers = new Headers({
    "Content-Type": "application/json",
    "Accept": "application/json"
  });

  async getTodoItems(): Promise<ApiResponseWithType<TodoItem[]>> {
    const data = await fetch(this.url);
    return (await data.json()) ?? {};
  } 

  async getTodoItem(id: string): Promise<ApiResponseWithType<TodoItem>> {
    const data = await fetch(`${this.url}/${id}`);
    return (await data.json()) ?? {};
  }

  async updateTodoItem(id: string, todo: TodoItem): Promise<ApiResponseWithType<Number>> {
    const resp = await fetch(`${this.url}/${id}`, {
      method: 'PUT',
      headers: this.headers,
      body: JSON.stringify(todo)
    });
    return (await resp.json()) ?? {};
  }

  async addTodoItem(todo: TodoItemAdd): Promise<ApiResponseWithType<Number>> {
    const resp = await fetch(`${this.url}`, {
      method: 'POST',
      headers: this.headers,
      body: JSON.stringify(todo)
    });
    return (await resp.json()) ?? {};
  }

  async todoItemDescriptionExists(description: string): Promise<ApiResponseWithType<boolean>> {
    const resp = await fetch(`${this.url}/exist?description=${encodeURIComponent(description)}`, {
      method: 'POST',
      headers: this.headers,      
    });
    return (await resp.json()) ?? {};
  }
}
