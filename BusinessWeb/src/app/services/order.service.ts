import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Order } from '@stripe/stripe-js';
import { OrderItem } from '../models/orderItem';


@Injectable({
  providedIn: 'root'
})
export class OrderService {

  private apiUrl = `${environment.orderItemsApiUrl}`;

  constructor(private http: HttpClient) { }

  getOrderItems() {
    return this.http.get(this.apiUrl);
  }

  getOrderItemById(id: string) {
    return this.http.get(`${this.apiUrl}/${id}`);
  }

  createOrderItem(orderItem: OrderItem) {
    return this.http.post(this.apiUrl, orderItem);
  }

  updateOrderItem(id: string, orderItem: OrderItem) {
    return this.http.put(`${this.apiUrl}/${id}`, orderItem);
  }

  deleteOrderItem(id: string) {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }
}
