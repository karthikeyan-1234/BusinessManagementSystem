import { Component } from '@angular/core';
import { RouterOutlet, RouterLinkActive } from '@angular/router';
import { CommonModule } from '@angular/common';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatListModule } from '@angular/material/list';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { Router, RouterModule } from '@angular/router';


@Component({
  selector: 'app-menu',
  standalone: true,
  imports: [RouterOutlet, CommonModule, MatSidenavModule, MatListModule, MatToolbarModule, 
    MatIconModule, MatButtonModule, RouterLinkActive, RouterModule],
  templateUrl: './menu.component.html',
  styleUrls: ['./menu.component.css']
})
export class MenuComponent {

  isSideNavOpen = true;

  toggleSideNav() {
    this.isSideNavOpen = !this.isSideNavOpen;
  }

  logOut() {
    // Implement your logout logic here
  }

}
