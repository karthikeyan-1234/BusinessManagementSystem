import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { PaymentIntentCreateRequest } from '../models/paymentIntentCreateRequest';

@Injectable({
  providedIn: 'root'
})
export class CheckoutService {

  constructor(private http: HttpClient) { }

  createPaymentIntent(paymentIntent: PaymentIntentCreateRequest) {
    return this.http.post<{clientSecret: string}>('https://localhost:7017/api/Payments/create-payment-intent'
      , paymentIntent);
  }
}
