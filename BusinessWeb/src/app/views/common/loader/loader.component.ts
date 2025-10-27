import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { LoaderService } from '../../../services/loader.service';

@Component({
  selector: 'app-loader',
  imports: [CommonModule],
  templateUrl: './loader.component.html',
  styleUrl: './loader.component.css'
})
export class LoaderComponent {

   // Directly expose the signal for binding

  constructor(private loaderService: LoaderService) { 
  }

  get loading() {
    return this.loaderService.loading();
  }


}
