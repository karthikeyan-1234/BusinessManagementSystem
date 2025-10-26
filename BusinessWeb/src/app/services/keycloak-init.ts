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
      bearerExcludedUrls: [], // VERY IMPORTANT: ensures /api calls include token

    }).then(() => {
      // ✅ Get the username and roles from Keycloak tokens
      const keycloakInstance = keycloak.getKeycloakInstance();

      // The ID Token is best for user identity details (like username, email)
      const idTokenClaims = keycloakInstance.idTokenParsed;
      // The Access Token is best for authorization details (like roles)
      const accessTokenClaims = keycloakInstance.tokenParsed;

      let username: string | null = null;
      let roles: string[] = [];

      // --- 1. Get Username ---
      // Prioritize the 'preferred_username' claim
      if (idTokenClaims && idTokenClaims['preferred_username']) {
        username = idTokenClaims['preferred_username'];
      } else if (accessTokenClaims && accessTokenClaims['preferred_username']) {
        username = accessTokenClaims['preferred_username'];
      } else if (accessTokenClaims && accessTokenClaims['sub']) {
        // Fallback to the subject ID
        username = accessTokenClaims['sub'];
      }

      if (username) {
        localStorage.setItem('username', username);
        console.log(`User logged in: ${username}`);
      } else {
        console.warn('Could not find preferred_username or sub in Keycloak tokens.');
      }

      // --------------------------------------------------------------------------------------------------
      // --- 2. Get Roles from Access Token ---
      // --------------------------------------------------------------------------------------------------

      // Get Client/Resource Roles (using your api-app clientId)
      const clientRoles: string[] = accessTokenClaims?.resource_access?.['api-app']?.roles || [];
      roles = [...roles, ...clientRoles];
      if (clientRoles.length) {
        console.log('Client roles found:', clientRoles);
      }
      
  
      if (roles.length > 0) {
        // Filter out duplicates and save the combined list of unique roles
        const uniqueRoles = Array.from(new Set(roles));
        localStorage.setItem('roles', JSON.stringify(uniqueRoles));
        console.log(`User roles stored: ${uniqueRoles.join(', ')}`);
      } else {
        console.warn('No roles found in Access Token claims.');
      }
      // --------------------------------------------------------------------------------------------------

    });
}