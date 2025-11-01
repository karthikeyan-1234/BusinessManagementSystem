// src/environments/environment.ts
export const environment = {
  production: false,
  stripePublishableKey: 'pk_test_51NNIIMSHXTQ38KZZ9xN8WOddbu2pYwPvqPUMe3PYTAMxv8wfhPUzk3kVsJQcwNiz7ezLz3SSIRWpZTEWDr297eRB00K3QGopZV', // Roll this key immediately!
  productsApiUrl: 'https://localhost:7030/api/products',
  orderItemsApiUrl: 'https://localhost:7299/api/orderItems',
  ordersApiUrl: 'https://localhost:7299/api/order',
  purchaseApiUrl: 'https://localhost:7275/api/purchase',
  appUrl: 'http://localhost:4200',
  rightSideDialogOptions: {
      width: '350px',
      height: '100vh',
      maxWidth: '450px',
      maxHeight: '100vh',
      position: { right: '0' },
      panelClass: ['right-side-dialog'],
      hasBackdrop: true,
      backdropClass: 'right-side-backdrop',
      disableClose: true, // Prevent closing on backdrop click
      data: {}
    }
};