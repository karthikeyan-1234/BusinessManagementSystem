import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ProductsService {

  private apiUrl = `${environment.productsApiUrl}`;

  constructor(private http: HttpClient) { }

  getProducts() {
    return this.http.get(this.apiUrl);
  }

  getProductById(id: string) {
    return this.http.get(`${this.apiUrl}/${id}`);
  }

  createProduct(product: any) {
    return this.http.post(this.apiUrl, product);
  }

  updateProduct(id: string, product: any) {
    return this.http.put(`${this.apiUrl}/${id}`, product);
  }

  deleteProduct(id: string) {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }

  addProduct(product: any) {
    return this.http.post(this.apiUrl, product);
  }
}
