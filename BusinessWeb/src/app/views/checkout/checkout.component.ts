import { Component } from '@angular/core';
import { loadStripe, Stripe, StripeElements } from '@stripe/stripe-js';
import { environment } from '../../../environments/environment';
import { CheckoutService } from '../../services/checkout.service';
import { PaymentIntentCreateRequest } from '../../models/paymentIntentCreateRequest';
import { v4 as uuidv4 } from 'uuid';
import { Router } from '@angular/router';


@Component({
  selector: 'app-checkout',
  standalone: true,
  imports: [],
  templateUrl: './checkout.component.html',
  styleUrl: './checkout.component.css'
})
export class CheckoutComponent {
  private stripe: Stripe | null = null;
  private elements: StripeElements | null = null
  private clientSecret: string = '';

  constructor(private checkoutService: CheckoutService, private router: Router) {}

  async ngOnInit() {
  this.stripe = await loadStripe(environment.stripePublishableKey);

  // 1️⃣ Create PaymentIntent
  const paymentIntentRequest: PaymentIntentCreateRequest = {
    amount: 1999, // $19.99
    currency: 'inr',
    orderId: uuidv4()
  };

  this.checkoutService.createPaymentIntent(paymentIntentRequest)
    .subscribe(async (response) => {
      this.clientSecret = response.clientSecret;

      if (this.stripe) {
        // 2️⃣ Initialize Elements only once
        this.elements = this.stripe.elements({ clientSecret: this.clientSecret });
        const paymentElement = this.elements.create('payment');
        paymentElement.mount('#payment-element');
      }
    });
}

async handleSubmit() {

  if (!this.stripe || !this.elements) {
    console.error('Stripe not initialized');
    return;
  }

  const { error, paymentIntent } = await this.stripe.confirmPayment({
    elements: this.elements,
    confirmParams: {
      return_url: 'http://localhost:4200/payment-success', // keep this for safety
    },
    redirect: 'if_required', // prevent reloads
  });

  if (error) {
    console.error('❌ Payment failed:', error.message);
    alert(`Payment failed: ${error.message}`);
  } else if (paymentIntent && paymentIntent.status === 'succeeded') {
    console.log('✅ Payment succeeded:', paymentIntent);
    alert('Payment successful!');
    this.router.navigate(['/payment-success']);
  } else {
    console.log('⏳ Payment pending:', paymentIntent);
    alert('Payment pending. Please check your payment method.');
  }

}
  
}