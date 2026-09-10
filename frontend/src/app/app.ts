import { Component } from '@angular/core';
import { CustomerFormComponent } from './features/customer-form/customer-form.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CustomerFormComponent],
  templateUrl: './app.html',
  styleUrl: './app.scss',
})
export class App {}
