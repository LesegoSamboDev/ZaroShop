import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

/**
 * Interface representing the Category DTO structure.
 * Includes optional 'children' to support the hierarchical tree view.
 */
export interface Category {
  id: number;
  name: string;
  description?: string;
  parentCategoryId?: number | null;
  children?: Category[];
}

/**
 * Interface representing the request payload for creating a category.
 */
export interface CategoryRequest {
  name: string;
  description?: string;
  parentCategoryId?: number | null;
}

@Injectable({
  providedIn: 'root'
})
export class CategoryService {
  private apiUrl = '/api/categories';

  constructor(private http: HttpClient) { }

  getCategories(): Observable<Category[]> {
    return this.http.get<Category[]>(this.apiUrl);
  }

  getCategoryTree(): Observable<Category[]> {
    return this.http.get<Category[]>(`${this.apiUrl}/tree`);
  }

  getCustomSerializedTree(): Observable<Category[]> {
    return this.http.get<Category[]>(`${this.apiUrl}/tree/custom`);
  }

  createCategory(request: CategoryRequest): Observable<Category> {
    return this.http.post<Category>(this.apiUrl, request);
  }
}
