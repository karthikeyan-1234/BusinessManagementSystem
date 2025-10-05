import { Component, VERSION } from '@angular/core';
import { MatToolbarModule  } from '@angular/material/toolbar';
import { MatIconModule } from '@angular/material/icon';
import {MatButtonModule} from '@angular/material/button';
import {MatTableModule} from '@angular/material/table';
import { CommonModule } from '@angular/common';

import { MatSidenavModule } from '@angular/material/sidenav';
import { MatListModule } from '@angular/material/list';
import {  RouterModule } from '@angular/router';
import { MenuComponent } from './views/common/menu/menu.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [MenuComponent, MatToolbarModule, MatIconModule, MatButtonModule, MatTableModule, 
    CommonModule, MatSidenavModule, MatListModule, RouterModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'BusinessWeb';
}
