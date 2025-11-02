import { Injectable, signal, computed } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { environment } from '../../environments/environment'; // Assuming you have environment variables

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  // Signal to hold the raw notification count received from the server
  private _rawNotificationCount = signal<number>(0);
  
  // Public signal to expose the count, computed to prevent negative values
  readonly notificationCount = computed(() => Math.max(0, this._rawNotificationCount()));

  private hubConnection!: signalR.HubConnection;
  private connectionEstablished = false;
  private readonly hubUrl = `${environment.notificationUrl}/notificationHub`; // e.g., 'http://localhost:5000/notificationHub'

  constructor() {
    this.startConnection();
  }

  /**
   * Initializes and starts the SignalR connection.
   */
  public startConnection = () => {
    if (this.connectionEstablished) {
        return;
    }

    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(this.hubUrl, {
        // Example of passing a token, if needed for authentication
        // accessTokenFactory: () => localStorage.getItem('authToken') || '' 
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection
      .start()
      .then(() => {
        console.log('SignalR Connection started');
        this.connectionEstablished = true;
        this.addNotificationListener();
      })
      .catch(err => {
        console.error('Error while starting SignalR connection: ' + err);
        // Implement exponential backoff for retries if automatic reconnect is insufficient
        setTimeout(this.startConnection, 5000); 
      });
  }

  /**
   * Sets up the listener for the server method 'ReceiveNotificationCount'.
   */
  private addNotificationListener = () => {
    // The server method defined in .NET needs to match this string
    this.hubConnection.on('ReceiveNotificationCount', (count: number) => {
      console.log('Notification received, new count:', count);
      this._rawNotificationCount.set(count);
    });
  }

  /**
   * Optional: Allows the component to manually reset the count (e.g., when the user opens the notification panel).
   */
  public resetCount() {
    this._rawNotificationCount.set(0);
    // In a real app, you would send a command back to the server here
    // this.hubConnection.invoke('MarkAllNotificationsAsRead');
  }
}
