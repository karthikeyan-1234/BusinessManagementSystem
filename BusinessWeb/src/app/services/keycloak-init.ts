import Keycloak from 'keycloak-js';
import { KeycloakService } from 'keycloak-angular';

export function initializeKeycloak(keycloak: KeycloakService): () => Promise<void> {
  return (): Promise<void> =>
    keycloak.init({
      config: {
        url: 'http://localhost:8080',
        realm: 'master',
        clientId: 'angular-app',
      },
      initOptions: {
        onLoad: 'login-required', //'check-sso',
        //silentCheckSsoRedirectUri: window.location.origin + '/assets/silent-check-sso.html'
      },
    }).then(() => {});
}