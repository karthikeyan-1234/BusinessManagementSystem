import { OrderStatus } from "../orderItem";

export interface CreatePurchase {
    purchaseDate: string;
    paymentTransactionId?: string;
    state: OrderStatus;
}
