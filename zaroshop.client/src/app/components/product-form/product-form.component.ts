import { Component, OnInit, NgZone, ChangeDetectorRef } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ProductService } from '../../services/product.service';
import { CategoryService, Category } from '../../services/category.service';

@Component({
  selector: 'app-product-form',
  standalone: false,
  templateUrl: './product-form.component.html',
})
export class ProductFormComponent implements OnInit {
  productForm!: FormGroup;
  isEditMode = false;
  productId?: number;
  categories: Category[] = [];

  constructor(
    private fb: FormBuilder,
    private productService: ProductService,
    private categoryService: CategoryService,
    private route: ActivatedRoute,
    private router: Router,
    private ngZone: NgZone,
    private cd: ChangeDetectorRef
  ) {
    this.initForm();
  }

  ngOnInit(): void {
    this.loadCategories();

    this.productId = this.route.snapshot.params['id'];
    if (this.productId) {
      this.isEditMode = true;
      this.loadProductData(this.productId);
    }
  }

  private initForm() {
    this.productForm = this.fb.group({
      id: [0], // Added for the update request DTO
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
          this.cd.detectChanges();
        });
      },
      error: (err) => console.error('Error fetching industries:', err)
    });
  }

  loadProductData(id: number) {
    this.productService.getById(id).subscribe({
      next: (product) => {
        this.ngZone.run(() => {
          // Flattening the object to match the form fields 
          // (Backend now returns categoryName, but form needs categoryId)
          this.productForm.patchValue({
            ...product,
            categoryId: product.categoryId // Ensure we map the ID for the dropdown
          });
          this.cd.detectChanges();
        });
      }
    });
  }

  onSubmit() {
    if (this.productForm.invalid) return;

    const productData = {
      ...this.productForm.value,
      categoryId: Number(this.productForm.value.categoryId)
    };

    // Explicitly type action$ as Observable<any>
    const action$: import('rxjs').Observable<any> = (this.isEditMode && this.productId)
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
