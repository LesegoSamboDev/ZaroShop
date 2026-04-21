import { Component, OnInit } from '@angular/core';
import { ProductService, Product } from '../../services/product.service';
import { Subject, debounceTime, distinctUntilChanged, switchMap } from 'rxjs';

@Component({
  selector: 'app-product-list',
  templateUrl: './product-list.component.html'
})
export class ProductListComponent implements OnInit {
  products: Product[] = [];
  loading = false;
  private searchTerms = new Subject<string>();

  constructor(private productService: ProductService) { }

  ngOnInit(): void {
    this.loadAllProducts();

    // Setup reactive search
    this.searchTerms.pipe(
      debounceTime(300), // Wait 300ms after last keystroke
      distinctUntilChanged(), // Only search if text changed
      switchMap(term => {
        this.loading = true;
        return term ? this.productService.searchProducts(term) : this.productService.getProducts();
      })
    ).subscribe(results => {
      this.products = results;
      this.loading = false;
    });
  }

  loadAllProducts() {
    this.loading = true;
    this.productService.getProducts().subscribe(data => {
      this.products = data;
      this.loading = false;
    });
  }

  onSearch(event: Event) {
    const term = (event.target as HTMLInputElement).value;
    this.searchTerms.next(term);
  }

  filterByCategory(id: number) {
    this.loading = true;
    this.productService.getProducts(id).subscribe(data => {
      this.products = data;
      this.loading = false;
    });
  }

  handleSearch(term: string): void {
    this.loading = true;
    this.productService.searchProducts(term).subscribe({
      next: (results) => {
        this.products = results;
        this.loading = false;
      },
      error: () => this.loading = false
    });
  }
}
