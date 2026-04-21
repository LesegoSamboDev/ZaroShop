import { HttpClientModule } from '@angular/common/http';
import { NgModule, provideBrowserGlobalErrorListeners } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing-module';
import { App } from './app';
import { CommonModule } from '@angular/common';
import { ProductListComponent } from './components/product-list/product-list.component';
import { ProductFormComponent } from './components/product-form/product-form.component';
import { SearchBarComponent } from './components/search-bar/search-bar.component';
import { ReactiveFormsModule } from '@angular/forms';
import { CategoryManagementComponent } from './components/category-management/category-management.component';

@NgModule({
  declarations: [
    App,
    ProductListComponent,
    ProductFormComponent,
    SearchBarComponent,
    CategoryManagementComponent,
  ],
  imports: [BrowserModule, HttpClientModule, AppRoutingModule, CommonModule, ReactiveFormsModule],
  providers: [provideBrowserGlobalErrorListeners()],
  bootstrap: [App],
})
export class AppModule {}
