import { HttpClientModule } from '@angular/common/http';
import { NgModule, provideBrowserGlobalErrorListeners } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing-module';
import { App } from './app';
import { CommonModule } from '@angular/common';
import { ProductListComponent } from './components/product-list.component/product-list.component';
import { ProductFormComponent } from './components/product-form.component/product-form.component';
import { SearchBarComponent } from './components/search-bar/search-bar.component';

@NgModule({
  declarations: [App, ProductListComponent, ProductFormComponent, SearchBarComponent],
  imports: [BrowserModule, HttpClientModule, AppRoutingModule, CommonModule],
  providers: [provideBrowserGlobalErrorListeners()],
  bootstrap: [App],
})
export class AppModule {}
