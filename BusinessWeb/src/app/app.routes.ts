import { Routes } from '@angular/router';
import { ProductsComponent } from './views/products/products.component';
import { InventoryComponent } from './views/inventory/inventory.component';
import { OrdersComponent } from './views/orders/orders.component';
import { PaymentsComponent } from './views/payments/payments.component';
import { CheckoutComponent } from './views/checkout/checkout.component';
import { PaymentSuccessComponent } from './views/payment-success/payment-success.component';

export const routes: Routes = [
    { path: '', redirectTo: '/products', pathMatch: 'full' },
    { path: 'products', component: ProductsComponent },
    { path: 'inventory', component: InventoryComponent },
    { path: 'orders', component: OrdersComponent },
    { path: 'payments', component: PaymentsComponent },
    { path: 'checkout', component: CheckoutComponent },
    { path: 'payment-success', component: PaymentSuccessComponent },
];
