import { Component } from '@angular/core';
import { RouterOutlet, RouterLinkActive } from '@angular/router';
import { CommonModule } from '@angular/common';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatListModule } from '@angular/material/list';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../../services/auth.service';
import { LoaderComponent } from '../loader/loader.component';


@Component({
  selector: 'app-menu',
  standalone: true,
  imports: [RouterOutlet, CommonModule, MatSidenavModule, MatListModule, MatToolbarModule, 
    MatIconModule, MatButtonModule, RouterLinkActive, RouterModule, LoaderComponent],
  templateUrl: './menu.component.html',
  styleUrls: ['./menu.component.css']
})
export class MenuComponent {

  username: string | null = localStorage.getItem('username');

  constructor(private authService: AuthService, private router: Router) {
    this.username = localStorage.getItem('username');

    // Redirect to login if username is not found
    if (!this.username) {
      this.authService.logout();
    }
  }

  isSideNavOpen = true;

  toggleSideNav() {
    this.isSideNavOpen = !this.isSideNavOpen;
  }

  logOut() {
    this.authService.logout();
  }

}
