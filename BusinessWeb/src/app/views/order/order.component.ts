import { Component } from '@angular/core';
import { Order } from '../../models/order';
import { OrderService } from '../../services/order.service';
import { MatTableDataSource } from '@angular/material/table';
import { MatTableModule } from '@angular/material/table';
import { CommonModule } from '@angular/common';
import {MatButtonModule} from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { FormsModule } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { MatDialog } from '@angular/material/dialog';
import { environment } from '../../../environments/environment';
import { AddOrderComponent } from './add-order/add-order.component';

@Component({
  selector: 'app-order',
  standalone: true,
  imports: [CommonModule, MatTableModule, MatButtonModule, MatIconModule, FormsModule, MatInputModule],
  templateUrl: './order.component.html',
  styleUrls: ['./order.component.css']
})
export class OrderComponent {

  orders: Order[] = [];
  displayedColumns: string[] = ['id', 'customerName', 'orderDate', 'status', 'actions'];
  dataSource: MatTableDataSource<Order> = new MatTableDataSource<Order>(this.orders);
  editedOrder: Order | null = null;

  constructor(private orderService: OrderService, private dialog: MatDialog) {

    this.updateOrderList();
  }


    updateOrderList() {
      this.orderService.getOrders().subscribe((data: any) => {
        this.orders = data;
        this.dataSource.data = this.orders;
      });
    }

    setEditingOrder(order: Order | null) {
      this.editedOrder = order;
    }

    cancelEdit() {
      this.editedOrder = null;
    }

    saveEdit(order: Order) {
      this.orderService.updateOrder(order).subscribe(() => {
        this.editedOrder = null;
        this.updateOrderList();
      });
    }

    openAddDialog() {
      const dialogRef = this.dialog.open(AddOrderComponent, environment.rightSideDialogOptions);
  
      dialogRef.afterClosed().subscribe(result => {
          this.updateOrderList();
      });
    }

    deleteOrder(orderId: number) {
      this.orderService.deleteOrder(orderId.toString()).subscribe(() => {
        this.updateOrderList();
      });
    }


}
