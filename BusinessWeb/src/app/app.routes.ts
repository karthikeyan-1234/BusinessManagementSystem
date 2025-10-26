import { Routes } from '@angular/router';
import { ProductsComponent } from './views/products/products.component';
import { InventoryComponent } from './views/inventory/inventory.component';
import { OrderItemsComponent } from './views/orderItems/orderItems.component';
import { PaymentsComponent } from './views/payments/payments.component';
import { CheckoutComponent } from './views/checkout/checkout.component';
import { PaymentSuccessComponent } from './views/payment-success/payment-success.component';
import { OrderComponent } from './views/order/order.component';
import { KeycloakGuard } from './auth/keycloak.guard';

export const routes: Routes = [
    { path: '', redirectTo: '/products', pathMatch: 'full' },
    { path: 'products', component: ProductsComponent, canActivate: [KeycloakGuard] },
    { path: 'inventory', component: InventoryComponent, canActivate: [KeycloakGuard] },
    { path: 'orders', component: OrderComponent, canActivate: [KeycloakGuard] },
    { path: 'payments', component: PaymentsComponent, canActivate: [KeycloakGuard] },
    { path: 'checkout', component: CheckoutComponent, canActivate: [KeycloakGuard] },
    { path: 'payment-success', component: PaymentSuccessComponent, canActivate: [KeycloakGuard] },
];
