import Keycloak from 'keycloak-js';
import { KeycloakAngularModule,KeycloakService } from 'keycloak-angular';

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
       bearerExcludedUrls: [], // VERY IMPORTANT: ensures /api calls include token
       
    }).then(() => {
      // ✅ Get the username from Keycloak token
      const keycloakInstance = keycloak.getKeycloakInstance();
      
      // Get the ID Token claims (often contains displayable user info)
      const idTokenClaims = keycloakInstance.idTokenParsed; 

      let username: string | null = null;

      if (idTokenClaims && idTokenClaims['preferred_username']) {
          username = idTokenClaims['preferred_username'];
          console.log('Username found in ID token claims.');
      }

      if (username) {
        localStorage.setItem('username', username);
        console.log(`User logged in: ${username}`);
      } else {
        console.warn('Could not find preferred_username or sub in Keycloak tokens.');
      }
    });
}