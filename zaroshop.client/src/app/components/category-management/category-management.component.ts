import { Component, OnInit, ChangeDetectionStrategy, ChangeDetectorRef } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { CategoryService, Category } from '../../services/category.service';

@Component({
  selector: 'app-category-management',
  standalone: false,
  templateUrl: './category-management.component.html',
  styleUrls: ['./category-management.component.css'],
  // Use OnPush to prevent unnecessary change detection cycles
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class CategoryManagementComponent implements OnInit {
  categories: Category[] = [];
  categoryTree: Category[] = [];
  categoryForm: FormGroup;
  expandedNodes = new Set<number>();

  constructor(
    private categoryService: CategoryService,
    private fb: FormBuilder,
    private cdr: ChangeDetectorRef // Inject ChangeDetectorRef
  ) {
    this.categoryForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(2)]],
      description: [''],
      parentCategoryId: [null]
    });
  }

  ngOnInit(): void {
    this.loadData();
  }

  loadData(): void {
    // We use forkJoin or individual subscribes. 
    // Since we are in OnPush mode, we must call markForCheck() after data arrives.
    this.categoryService.getCategories().subscribe({
      next: (data) => {
        this.categories = data;
        this.cdr.markForCheck(); // Notify Angular to check this component
      },
      error: (err) => console.error('Error fetching categories:', err)
    });

    this.categoryService.getCategoryTree().subscribe({
      next: (data) => {
        this.categoryTree = data;
        this.cdr.markForCheck(); // Notify Angular to check this component
      },
      error: (err) => console.error('Error fetching category tree:', err)
    });
  }

  onSubmit(): void {
    if (this.categoryForm.valid) {
      this.categoryService.createCategory(this.categoryForm.value).subscribe({
        next: () => {
          this.categoryForm.reset({ parentCategoryId: null });
          this.loadData();
          // loadData calls markForCheck, but we call it here too for the form reset
          this.cdr.markForCheck();
        },
        error: (err) => console.error('Error creating category:', err)
      });
    }
  }

  toggleNode(nodeId: number): void {
    if (this.expandedNodes.has(nodeId)) {
      this.expandedNodes.delete(nodeId);
    } else {
      this.expandedNodes.add(nodeId);
    }
    // Since this is a UI-only change not triggered by an Input/Output, 
    // we manually trigger detection.
    this.cdr.markForCheck();
  }

  isExpanded(nodeId: number): boolean {
    return this.expandedNodes.has(nodeId);
  }

  onDelete(id: number): void {
    if (confirm('Are you sure?')) {
      console.log('Delete category:', id);
      // If deleting via service, remember to call this.cdr.markForCheck() in the callback
    }
  }
}
