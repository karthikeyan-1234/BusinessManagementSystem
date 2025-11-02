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
import { MatBadgeModule } from '@angular/material/badge';
import { NotificationService } from '../../../services/notification.service';


@Component({
  selector: 'app-menu',
  standalone: true,
  imports: [RouterOutlet, CommonModule, MatSidenavModule, MatListModule, MatToolbarModule, 
    MatIconModule, MatButtonModule, RouterLinkActive, RouterModule, LoaderComponent, MatBadgeModule],
  templateUrl: './menu.component.html',
  styleUrls: ['./menu.component.css']
})
export class MenuComponent {
openNotifications() {
throw new Error('Method not implemented.');
}

  username: string | null = localStorage.getItem('username');

  constructor(private authService: AuthService, private router: Router, public notify: NotificationService) {
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
