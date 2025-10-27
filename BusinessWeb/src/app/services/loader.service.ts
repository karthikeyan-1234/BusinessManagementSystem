// src/app/core/services/loader.service.ts
import { Injectable, signal } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class LoaderService {
  // Angular 16+ signal-based approach (preferred)
  loading = signal(false);

  private activeRequests = 0;

  show(): void {
    this.activeRequests++;
    this.loading.set(true);
  }

  hide(): void {
    this.activeRequests--;
    if (this.activeRequests <= 0) {
      this.activeRequests = 0;
      this.loading.set(false);
    }
  }
}
