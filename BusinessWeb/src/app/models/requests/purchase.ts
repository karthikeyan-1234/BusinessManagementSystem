import { OrderStatus } from "../orderItem";

export interface Purchase {
    id: string;
    purchaseDate: string;
    paymentTransactionId?: string;
    state: OrderStatus;
}