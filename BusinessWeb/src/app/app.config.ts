import { APP_INITIALIZER, ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { HTTP_INTERCEPTORS, provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';

import {provideNativeDateAdapter} from '@angular/material/core';
import { KeycloakService } from 'keycloak-angular';
import { initializeKeycloak } from './services/keycloak-init';
import { AuthInterceptor } from './auth/auth.interceptor';
import { UnauthorizedInterceptor } from './auth/unauthorized.interceptor';
import { LoaderInterceptor } from './auth/loader.interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }), 
    provideRouter(routes),
    KeycloakService,
    {
      provide: APP_INITIALIZER,
      useFactory: initializeKeycloak,
      deps: [KeycloakService],
      multi: true
    },
    provideHttpClient(withInterceptorsFromDi()),
    provideNativeDateAdapter(),

    {
      provide: HTTP_INTERCEPTORS,
      useClass: LoaderInterceptor,
      multi: true
    },

    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true
    },

    {
      provide: HTTP_INTERCEPTORS,
      useClass: UnauthorizedInterceptor,
      multi: true
    }
  ]
};
