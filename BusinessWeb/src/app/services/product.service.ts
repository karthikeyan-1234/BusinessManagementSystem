import { Injectable } from '@angular/core';
import { Product } from '../models/product';

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  private products: Product[] = [
    { productId: '1', name: 'Laptop', description: 'A high-performance laptop', price: 1200, stock: 10 },
    { productId: '2', name: 'Smartphone', description: 'A latest model smartphone', price: 800, stock: 5 },
    { productId: '3', name: 'Headphones', description: 'Noise-cancelling headphones', price: 200, stock: 15 }
  ];

  constructor() { }

  addProduct(product: Product) {
    this.products.push(product);
  }

  getProducts(): Product[] {
    return this.products;
  }

  getProductById(productId: string): Product | undefined {
    return this.products.find(p => p.productId === productId);
  }

  updateProduct(updatedProduct: Product) {
    const index = this.products.findIndex(p => p.productId === updatedProduct.productId);
    if (index !== -1) {
      this.products[index] = updatedProduct;
    } else {
      throw new Error('Product not found');
    }
  }

  deleteProduct(productId: string) {
    this.products = this.products.filter(p => p.productId !== productId);
  }
}
