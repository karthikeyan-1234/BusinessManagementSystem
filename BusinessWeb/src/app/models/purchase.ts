import { OrderStatus } from "./orderItem";

export interface Purchase {
    id: string;
    purchasedate: Date;
    paymentTransactionId: string;
    state: OrderStatus
}
