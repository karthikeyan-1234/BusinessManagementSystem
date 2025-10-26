import { Component } from '@angular/core';
import { Product } from '../../models/product';
import { ProductsService } from '../../services/products.service';
import { MatTableDataSource } from '@angular/material/table';
import { MatTableModule } from '@angular/material/table';
import { CommonModule } from '@angular/common';
import {MatButtonModule} from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { FormsModule } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { MatDialog } from '@angular/material/dialog';
import { AddProductComponent } from './add-product/add-product.component';
import { environment } from '../../../environments/environment';
import { KeycloakService } from 'keycloak-angular';

@Component({
  selector: 'app-products',
  standalone: true,
  imports: [MatTableModule, CommonModule, MatButtonModule, MatIconModule, FormsModule, MatInputModule],
  templateUrl: './products.component.html',
  styleUrl: './products.component.css'
})
export class ProductsComponent {

  products: Product[] = [];
  displayedColumns: string[] = ['name', 'price', 'stock', 'actions'];
  dataSource: MatTableDataSource<Product> = new MatTableDataSource(this.products);
  editedProduct: Product | null = null;

  constructor(private productsService: ProductsService, private dialog: MatDialog, private keycloak: KeycloakService) {
    this.updateProductList();
  }

  async ngOnInit() {
  const token = await this.keycloak.getToken();
  console.log('Access Token:', token);
}

  updateProductList() {
    this.productsService.getProducts().subscribe((data: any) => {
      this.products = data;
      this.dataSource.data = this.products;
    });
  }

  setEditingProduct(product: Product | null) {
    this.editedProduct = product ? { ...product } : null;
  }

  cancelEdit() {
    this.editedProduct = null;
  }

  saveEdit(product: Product) {
    this.productsService.updateProduct(product.productId, product).subscribe(() => {
      this.editedProduct = null;
      this.updateProductList();
    });
  }

  deleteProduct(productId: number) {
    this.productsService.deleteProduct(productId.toString()).subscribe(() => {
      this.updateProductList();
    });
  }

  openAddDialog() {
    const dialogRef = this.dialog.open(AddProductComponent, environment.rightSideDialogOptions);

    dialogRef.afterClosed().subscribe(result => {
        this.updateProductList();
    });
  }

}
