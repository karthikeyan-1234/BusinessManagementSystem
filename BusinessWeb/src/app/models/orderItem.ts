
export enum OrderStatus {
    Pending = 'Pending',
    Completed = 'Completed',
    Failed = 'Failed'
}

export interface OrderItem {
    orderId: string;
    productId: string;
    quantity: number;
    price: number;
    status: OrderStatus;
    paymentTransactionId?: string; // Optional
}