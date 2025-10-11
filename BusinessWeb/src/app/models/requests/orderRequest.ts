/*public record OrderRequest(string? customerName, DateTime orderDate, OrderStatus status);*/

import { OrderStatus } from "../orderItem";

export interface OrderRequest {
    customerName?: string;
    orderDate: Date;
    status: OrderStatus;
}
