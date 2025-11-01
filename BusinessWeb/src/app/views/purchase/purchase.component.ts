import { Component } from '@angular/core';
import { PurchaseService } from '../../services/purchase.service';
import { Purchase } from '../../models/requests/purchase';
import { MatIconModule } from '@angular/material/icon';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatTableDataSource, MatTable, MatTableModule } from '@angular/material/table';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatInput } from "@angular/material/input";
import { FormsModule } from '@angular/forms';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatSelect, MatOption } from "@angular/material/select";
import { LookUpIdInPipe } from '../../../shared/look-up-id-in.pipe';
import { AddPurchaseComponent } from './add-purchase/add-purchase.component';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-purchase',
  imports: [LookUpIdInPipe ,MatIconModule, MatDialogModule, MatTableModule, CommonModule, MatButtonModule, MatInput, FormsModule, MatDatepickerModule, MatSelect, MatOption],
  templateUrl: './purchase.component.html',
  styleUrls: ['./purchase.component.css']
})
export class PurchaseComponent {

  purchases: Purchase[] = [];
  dataSource = new MatTableDataSource<Purchase>();
  displayedColumns: string[] = ['purchaseDate', 'paymentTransactionId', 'state', 'actions'];
  editedPurchase: Purchase | null = null;
  purchaseStatuses: { id: number, name: string }[] = [];

  constructor(private purchaseService: PurchaseService, private dialog: MatDialog) {
    this.loadPurchases();
    this.loadPurchaseStatuses();
  }

  loadPurchases() {
    this.purchaseService.getAllPurchases().subscribe({
      next: (data) => {
        this.purchases = data;
        this.dataSource.data = this.purchases;
      },
      error: (error) => {
        console.error('Error fetching purchases:', error);
      }
    });
  }

  loadPurchaseStatuses() {
    this.purchaseService.getPurchaseStatuses().subscribe({
      next: (statuses) => {
        this.purchaseStatuses = statuses;
      },
      error: (error) => {
        console.error('Error fetching purchase statuses:', error);
      }
    });
  }

  openAddDialog() {
    this.dialog.open(AddPurchaseComponent,environment.rightSideDialogOptions).afterClosed().subscribe(result => {
      if(result != null && result == true)
      {
        this.loadPurchases();
      }
    })
  }

  cancelEdit() {
    this.editedPurchase = null;
  }
  saveEdit(purchase: Purchase) {
    if (this.editedPurchase) {
      this.purchaseService.updatePurchase(this.editedPurchase.id, purchase).subscribe({
        next: () => {
          this.loadPurchases();
          this.editedPurchase = null;
        },
        error: (error) => {
          console.error('Error saving purchase:', error);
        }
      });
    }
  }

  deletePurchase(id: string) {
    this.purchaseService.deletePurchase(id).subscribe({
      next: () => {
        this.loadPurchases();
      },
      error: (error) => {
        console.error('Error deleting purchase:', error);
      }
    });
  }

  setEditingPurchase(purchase: Purchase | null) {
    this.editedPurchase = purchase;
  }
}
