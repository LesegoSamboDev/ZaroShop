import { Component, OnInit, NgZone, ChangeDetectorRef } from '@angular/core';
import { ProductService, Product, ProductFilters, PaginatedResponse } from '../../services/product.service';
import { CategoryService, Category } from '../../services/category.service';
import { Subject, debounceTime, distinctUntilChanged } from 'rxjs';

@Component({
  selector: 'app-product-list',
  standalone: false,
  templateUrl: './product-list.component.html'
})
export class ProductListComponent implements OnInit {
  viewMode: 'table' | 'grid' = 'table';
  products: Product[] = [];
  categories: Category[] = [];
  loading = false;

  // Pagination Metadata
  totalItems = 0;
  totalPages = 0;

  private searchTerms = new Subject<string>();

  currentFilters: ProductFilters = {
    search: '',
    name: '',
    categoryId: undefined,
    minPrice: undefined,
    maxPrice: undefined,
    onlyInStock: false,
    pageNumber: 1, // Default to first page
    pageSize: 10   // Default page size
  };

  constructor(
    private productService: ProductService,
    private categoryService: CategoryService,
    private ngZone: NgZone,
    private cd: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this.loadCategories();
    this.applyFilters();

    this.searchTerms.pipe(
      debounceTime(300),
      distinctUntilChanged()
    ).subscribe(term => {
      this.currentFilters.search = term;
      this.currentFilters.pageNumber = 1; // Reset to page 1 on new search
      this.applyFilters();
    });
  }

  loadCategories() {
    this.categoryService.getCategories().subscribe({
      next: (data) => {
        this.ngZone.run(() => {
          this.categories = data;
          this.cd.detectChanges();
        });
      },
      error: (err) => console.error('Category load failed:', err)
    });
  }

  applyFilters() {
    this.loading = true;
    this.productService.getProducts(this.currentFilters).subscribe({
      next: (response: PaginatedResponse<Product>) => {
        this.ngZone.run(() => {
          // Extract data from the wrapper object
          this.products = response.items;
          this.totalItems = response.totalItems;
          this.totalPages = response.totalPages;

          this.loading = false;
          this.cd.markForCheck();
          this.cd.detectChanges();
        });
      },
      error: (err) => {
        this.ngZone.run(() => {
          console.error('Fetch error:', err);
          this.loading = false;
          this.cd.detectChanges();
        });
      }
    });
  }

  // Navigation helper for the template
  goToPage(page: number) {
    if (page >= 1 && page <= this.totalPages) {
      this.currentFilters.pageNumber = page;
      this.applyFilters();
    }
  }

  onSearch(term: string) {
    this.searchTerms.next(term);
  }

  updateCategory(id: string) {
    this.currentFilters.categoryId = id ? +id : undefined;
    this.currentFilters.pageNumber = 1; // Reset to page 1 when filter changes
    this.applyFilters();
  }

  updateStockFilter(onlyInStock: boolean) {
    this.currentFilters.onlyInStock = onlyInStock;
    this.currentFilters.pageNumber = 1; // Reset to page 1
    this.applyFilters();
  }

  loadAllProducts() {
    // Reset to initial state
    this.currentFilters = {
      search: '',
      name: '',
      pageNumber: 1,
      pageSize: 10
    };
    this.applyFilters();
  }

  onDelete(id: number): void {
    if (confirm('Are you sure you want to delete this product?')) {
      this.productService.deleteProduct(id).subscribe({
        next: () => this.applyFilters(),
        error: (err) => alert('Delete failed: ' + err.message)
      });
    }
  }
}
