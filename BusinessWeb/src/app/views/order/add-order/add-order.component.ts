import { Component, Inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Product } from '../../../models/product';
import { MAT_DIALOG_DATA, MatDialog, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { CommonModule } from '@angular/common';
import { MatIconModule } from "@angular/material/icon";
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { OrderService } from '../../../services/order.service';
import { ProductsService } from '../../../services/products.service';
import { MatSelectModule } from '@angular/material/select';
import {MatDatepickerModule} from '@angular/material/datepicker';



@Component({
  selector: 'app-add-order',
  standalone: true,
  imports: [CommonModule, MatDialogModule, MatIconModule, MatButtonModule, MatFormFieldModule, MatInputModule, FormsModule, MatSelectModule, MatDatepickerModule],
  templateUrl: './add-order.component.html',
  styleUrl: './add-order.component.css'
})
export class AddOrderComponent {

  order: any = { orderId: '', productId: '', quantity: 0, totalPrice: 0 };
  products: Product[] = [];

  constructor(@Inject(MAT_DIALOG_DATA) private data: any,
    private dialogRef: MatDialogRef<AddOrderComponent>, private orderService: OrderService, 
    private productService: ProductsService) {

    this.productService.getProducts().subscribe({
      next: (response) => {
        this.products = response;
      }
    });
  }

  onSubmit() {
    this.orderService.createOrder(this.order).subscribe({
      next: (response) => {
            this.dialogRef.close(this.order); // Close dialog and return the order
      },
      error: (error) => {
        console.error('Error adding order:', error);
      }
    });
    this.order = { orderId: '', productId: '', quantity: 0, totalPrice: 0 }; // Reset form
  }

  closeDialog(){
    this.dialogRef.close();
  }

}
