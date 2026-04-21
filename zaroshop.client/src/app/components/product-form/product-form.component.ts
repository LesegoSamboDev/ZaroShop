import { Component, OnInit, NgZone, ChangeDetectorRef } from '@angular/core'; // 1. Added Zone and CD
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ProductService } from '../../services/product.service';
import { CategoryService, Category } from '../../services/category.service'; // 2. Import Category Service

@Component({
  selector: 'app-product-form',
  standalone: false,
  templateUrl: './product-form.component.html',
})
export class ProductFormComponent implements OnInit {
  productForm!: FormGroup;
  isEditMode = false;
  productId?: number;
  categories: Category[] = []; // 3. Empty array for dynamic data

  constructor(
    private fb: FormBuilder,
    private productService: ProductService,
    private categoryService: CategoryService, // 4. Inject Category Service
    private route: ActivatedRoute,
    private router: Router,
    private ngZone: NgZone,
    private cd: ChangeDetectorRef
  ) {
    this.initForm();
  }

  ngOnInit(): void {
    // 5. Load categories and product data
    this.loadCategories();

    this.productId = this.route.snapshot.params['id'];
    if (this.productId) {
      this.isEditMode = true;
      this.loadProductData(this.productId);
    }
  }

  private initForm() {
    this.productForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(3)]],
      sku: ['', [Validators.required, Validators.pattern(/^[A-Z0-9-]+$/)]],
      price: [0, [Validators.required, Validators.min(0.01)]],
      quantity: [0, [Validators.required, Validators.min(0)]],
      categoryId: ['', Validators.required]
    });
  }

  private loadCategories() {
    this.categoryService.getCategories().subscribe({
      next: (data) => {
        this.ngZone.run(() => {
          this.categories = data;
          this.cd.detectChanges(); // Force dropdown to render
        });
      },
      error: (err) => console.error('Error fetching industries:', err)
    });
  }

  loadProductData(id: number) {
    this.productService.getById(id).subscribe(product => {
      this.ngZone.run(() => {
        this.productForm.patchValue(product);
        this.cd.detectChanges(); // Ensure values show up in inputs
      });
    });
  }

  onSubmit() {
    if (this.productForm.invalid) return;

    const productData = {
      ...this.productForm.value,
      categoryId: Number(this.productForm.value.categoryId)
    };

    const action$ = (this.isEditMode && this.productId)
      ? this.productService.updateProduct(this.productId, productData)
      : this.productService.createProduct(productData);

    action$.subscribe({
      next: () => {
        this.ngZone.run(() => this.router.navigate(['/products']));
      },
      error: (err) => {
        console.error('Save failed:', err);
        alert('Failed to save product.');
      }
    });
  }
}
