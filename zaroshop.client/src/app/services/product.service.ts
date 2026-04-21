import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Product {
  id: number;
  name: string;
  sku: string;
  price: number;
  quantity: number;
  categoryName: string;
  categoryId: number;
}

export interface ProductFilters {
  search?: string; // Unified search term
  name?: string;
  categoryId?: number;
  minPrice?: number;
  maxPrice?: number;
  onlyInStock?: boolean;
  pageNumber?: number; // Added for pagination
  pageSize?: number;
}

// 2. New interface to match the Backend's wrapper object
export interface PaginatedResponse<T> {
  totalItems: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  items: T[];
}

@Injectable({ providedIn: 'root' })
export class ProductService {
  private apiUrl = '/api/products';

  constructor(private http: HttpClient) { }

  getById(id: number): Observable<Product> {
    return this.http.get<Product>(`${this.apiUrl}/${id}`);
  }

  // Ensure categoryId is sent as a number to match the .NET DTO
  createProduct(product: any): Observable<Product> {
    const payload = { ...product, categoryId: Number(product.categoryId) };
    return this.http.post<Product>(this.apiUrl, payload);
  }

  updateProduct(id: number, product: any): Observable<void> {
    const payload = { ...product, categoryId: Number(product.categoryId) };
    return this.http.put<void>(`${this.apiUrl}/${id}`, payload);
  }

  getProducts(filters: ProductFilters = {}): Observable<PaginatedResponse<Product>> {
    let params = new HttpParams();

    if (filters.search) params = params.set('search', filters.search);
    if (filters.name) params = params.set('name', filters.name);
    if (filters.categoryId) params = params.set('categoryId', filters.categoryId.toString());
    if (filters.minPrice) params = params.set('minPrice', filters.minPrice.toString());
    if (filters.maxPrice) params = params.set('maxPrice', filters.maxPrice.toString());
    if (filters.onlyInStock) params = params.set('onlyInStock', 'true');

    // 4. Map the new pagination parameters
    if (filters.pageNumber) params = params.set('pageNumber', filters.pageNumber.toString());
    if (filters.pageSize) params = params.set('pageSize', filters.pageSize.toString());

    return this.http.get<PaginatedResponse<Product>>(this.apiUrl, { params });
  }

  searchProducts(query: string): Observable<PaginatedResponse<Product>> {
    return this.getProducts({ search: query, pageNumber: 1, pageSize: 10 });
  }

  deleteProduct(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
