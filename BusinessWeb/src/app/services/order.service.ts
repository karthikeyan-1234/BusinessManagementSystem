import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';


@Injectable({
  providedIn: 'root'
})
export class OrderService {

  private apiUrl = `${environment.ordersApiUrl}`;

  constructor(private http: HttpClient) { }

  getOrders() {
    return this.http.get(this.apiUrl);
  }

  getOrderById(id: string) {
    return this.http.get(`${this.apiUrl}/${id}`);
  }

  createOrder(order: any) {
    return this.http.post(this.apiUrl, order);
  }

  updateOrder(id: string, order: any) {
    return this.http.put(`${this.apiUrl}/${id}`, order);
  }

  deleteOrder(id: string) {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }
}
