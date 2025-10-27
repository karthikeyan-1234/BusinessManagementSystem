// src/app/core/interceptors/loader.interceptor.ts
import { Injectable } from '@angular/core';
import {
  HttpInterceptor,
  HttpRequest,
  HttpHandler,
  HttpEvent,
} from '@angular/common/http';
import { Observable, finalize } from 'rxjs';
import { LoaderService } from '../services/loader.service';

@Injectable()
export class LoaderInterceptor implements HttpInterceptor {
  constructor(private loaderService: LoaderService) {}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    // Show loader before sending request
    this.loaderService.show();

    return next.handle(req).pipe(
      finalize(() => {
        // Hide loader on response or error
        this.loaderService.hide();
      })
    );
  }
}
