import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { FormsModule } from "@angular/forms";
import { MatFormField, MatInputModule, MatLabel } from "@angular/material/input";
import { Purchase } from '../../../models/requests/purchase';
import { OrderStatus } from '../../../models/orderItem';
import { PurchaseService } from '../../../services/purchase.service';
import { MatButtonModule } from '@angular/material/button';
import { CreatePurchase } from '../../../models/requests/createPurchase';
import { MatSelect, MatOption } from "@angular/material/select";
import { MatSelectModule } from '@angular/material/select';
import { MatOptionModule } from '@angular/material/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-add-purchase',
  imports: [MatSelectModule, MatDialogModule, MatIconModule, FormsModule, 
    MatFormField, MatLabel, MatButtonModule, MatInputModule, MatSelect, MatOption, MatOptionModule, CommonModule],
  templateUrl: './add-purchase.component.html',
  styleUrl: './add-purchase.component.css'
})
export class AddPurchaseComponent {

  newPurchase: CreatePurchase = {purchaseDate: new Date().toISOString().slice(0, 10),paymentTransactionId: '',state: OrderStatus.Pending};
  purchaseStatuses: { id: number, name: string }[] = [];


  constructor(@Inject(MAT_DIALOG_DATA) private data: any, private dialogRef: MatDialogRef<AddPurchaseComponent>,
private purchaseService: PurchaseService){
  this.purchaseService.getPurchaseStatuses().subscribe(result => {
    this.purchaseStatuses = result
  })
}

  onSubmit() {
    this.purchaseService.createPurchase(this.newPurchase).subscribe((result) => {
      this.dialogRef.close(true);
    }, (err) => {
      this.dialogRef.close(false);
    })
  }
  closeDialog() {
    this.dialogRef.close(null);
  }

}
