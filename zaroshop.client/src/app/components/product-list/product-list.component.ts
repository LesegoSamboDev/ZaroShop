import { Component, OnInit, NgZone, ChangeDetectorRef } from '@angular/core';
import { ProductService, Product, ProductFilters } from '../../services/product.service';
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

  private searchTerms = new Subject<string>();
  currentFilters: ProductFilters = {
    name: '',
    categoryId: undefined,
    minPrice: undefined,
    maxPrice: undefined,
    onlyInStock: false
  };

  constructor(
    private productService: ProductService,
    private categoryService: CategoryService,
    private ngZone: NgZone,
    private cd: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this.loadCategories();
    this.applyFilters(); // Initial load

    this.searchTerms.pipe(
      debounceTime(300),
      distinctUntilChanged()
    ).subscribe(term => {
      this.currentFilters.search = term;
      this.applyFilters();
    });
  }

  loadCategories() {
    this.categoryService.getCategories().subscribe({
      next: (data) => {
        this.ngZone.run(() => {
          this.categories = data;

          this.cd.markForCheck(); 
          this.cd.detectChanges();
        });
      },
      error: (err) => console.error('Category load failed:', err)
    });
  }

  applyFilters() {
    this.loading = true;
    this.productService.getProducts(this.currentFilters).subscribe({
      next: (data) => {
        // 3. Wrap state updates in ngZone.run()
        this.ngZone.run(() => {
          this.products = data;
          this.loading = false;

          this.cd.markForCheck();
          this.cd.detectChanges();
          console.log('UI Force Updated. Products:', this.products.length);
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

  onSearch(term: string) {
    this.searchTerms.next(term);
  }

  updateCategory(id: string) {
    this.currentFilters.categoryId = id ? +id : undefined;
    this.applyFilters();
  }

  updateStockFilter(onlyInStock: boolean) {
    this.currentFilters.onlyInStock = onlyInStock;
    this.applyFilters();
  }

  loadAllProducts() {
    this.currentFilters = { name: '' };
    this.applyFilters();
  }

  onDelete(id: number): void {
    if (confirm('Are you sure you want to delete this product?')) {
      // Delete still needs a manual subscribe because it's an action, not a data stream
      this.productService.deleteProduct(id).subscribe({
        next: () => this.applyFilters(),
        error: (err) => alert('Delete failed: ' + err.message)
      });
    }
  }
}
