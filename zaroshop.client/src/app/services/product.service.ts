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

  getProducts(filters: ProductFilters = {}): Observable<Product[]> {
    let params = new HttpParams();

    // Mapping the search term to the new backend 'search' parameter
    if (filters.search) params = params.set('search', filters.search);

    if (filters.name) params = params.set('name', filters.name);
    if (filters.categoryId) params = params.set('categoryId', filters.categoryId.toString());
    if (filters.minPrice) params = params.set('minPrice', filters.minPrice.toString());
    if (filters.maxPrice) params = params.set('maxPrice', filters.maxPrice.toString());

    if (filters.onlyInStock) params = params.set('onlyInStock', 'true');

    return this.http.get<Product[]>(this.apiUrl, { params });
  }

  // This now just calls getProducts with the search filter
  searchProducts(query: string): Observable<Product[]> {
    return this.getProducts({ search: query });
  }

  deleteProduct(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
