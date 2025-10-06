export interface PaymentIntentCreateRequest {
    amount: number;
    currency?: string;
    orderId: string;
}   