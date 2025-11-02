import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { CreatePurchase } from '../models/requests/createPurchase';
import { Purchase } from '../models/requests/purchase';
import { Observable, of } from 'rxjs';
import { v4 as uuidv4 } from 'uuid';

@Injectable({
  providedIn: 'root'
})
export class PurchaseService {

  constructor(private http:HttpClient) { 
  }

  getAllPurchases(){
    return this.http.get<Purchase[]>(environment.purchaseApiUrl);
  }

  createPurchase(purchase:CreatePurchase){
    purchase.paymentTransactionId = uuidv4();
    return this.http.post<Purchase>(environment.purchaseApiUrl, purchase);
  }

  getPurchaseById(id: string) {
    return this.http.get<Purchase>(`${environment.purchaseApiUrl}/${id}`);
  }

  updatePurchase(id: string, purchase: Purchase) {
    console.log('Updating purchase with ID:', id, 'Data:', purchase);
    return this.http.put<Purchase>(`${environment.purchaseApiUrl}/${id}`, purchase);
  }

  deletePurchase(id: string): Observable<boolean> {
    return this.http.delete<boolean>(`${environment.purchaseApiUrl}/${id}`);
  }

  getPurchaseStatuses(): Observable<{ id: number, name: string }[]> {
    
    //Formulate the list of statuses based on OrderStatus enum
    const statuses = [
      { id: 0, name: 'Pending' },
      { id: 1, name: 'Completed' },
      { id: 2, name: 'Cancelled' }
    ];

    return of(statuses);
  }

}
