import { Component, Inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Product } from '../../../models/product';
import { MAT_DIALOG_DATA, MatDialog, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { CommonModule } from '@angular/common';
import { MatIconModule } from "@angular/material/icon";
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { ProductsService } from '../../../services/products.service';

@Component({
  selector: 'app-add-product',
  standalone: true,
  imports: [FormsModule, CommonModule, MatIconModule, MatDialogModule, MatButtonModule, MatFormFieldModule, MatInputModule],
  templateUrl: './add-product.component.html',
  styleUrl: './add-product.component.css'
})
export class AddProductComponent {
  product: Product = { productId: '', name: '', description: '', price: 0, stock: 0 };

  constructor(@Inject(MAT_DIALOG_DATA) private data: any,
    private dialogRef: MatDialogRef<AddProductComponent>, private productService: ProductsService) {
  }

  onSubmit() {
    console.log('Product added:', this.product);
    // Here you would typically send the product data to your backend service.

    this.productService.addProduct(this.product).subscribe({
      next: (response) => {
        console.log('Product successfully added:', response);
            this.dialogRef.close(this.product); // Close dialog and return the product
      },
      error: (error) => {
        console.error('Error adding product:', error);
        
      }
    });

    this.product = { productId: '', name: '', description: '', price: 0, stock: 0 }; // Reset form
  }


  closeDialog(){
    this.dialogRef.close();
  }

}
