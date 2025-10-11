import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { OrderItem } from '../models/orderItem';
import { Order } from '../models/order';
import { Observable } from 'rxjs';


@Injectable({
  providedIn: 'root'
})
export class OrderService {

  private orderItemsApiUrl = `${environment.orderItemsApiUrl}`;
  private ordersApiUrl = `${environment.ordersApiUrl}`;

  constructor(private http: HttpClient) { }

  createOrder(order: Order) {
    return this.http.post(`${environment.ordersApiUrl}`, order);
  }

  updateOrder(order: Order) {
    return this.http.put(`${environment.ordersApiUrl}`, order);
  }

  getOrderById(id: string) {
    return this.http.get(`${this.ordersApiUrl}/${id}`);
  }

  getOrders(): Observable<Order[]> {
    return this.http.get<Order[]>(this.ordersApiUrl);
  }

  updateOder( order: Order) {
    return this.http.put(`${this.ordersApiUrl}`, order);
  }

  deleteOrder(id: string) {
    return this.http.delete(`${this.ordersApiUrl}/${id}`);
  }

  getOrderItems() {
    return this.http.get(this.orderItemsApiUrl);
  }

  getOrderItemById(id: string) {
    return this.http.get(`${this.orderItemsApiUrl}/${id}`);
  }

  createOrderItem(orderItem: OrderItem) {
    return this.http.post(this.orderItemsApiUrl, orderItem);
  }

  updateOrderItem(id: string, orderItem: OrderItem) {
    return this.http.put(`${this.orderItemsApiUrl}/${id}`, orderItem);
  }

  deleteOrderItem(id: string) {
    return this.http.delete(`${this.orderItemsApiUrl}/${id}`);
  }
}
