import { OrderItem, OrderStatus } from "./orderItem";

export interface Order {
    id: string;
    customerName?: string;
    orderDate: Date;
    status: OrderStatus;
    orderItems?: OrderItem[];
}