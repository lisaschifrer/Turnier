import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatError } from '@angular/material/form-field';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';


@Component({
  selector: 'app-startseite',
  imports: [
    FormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatError,
    MatButtonModule,
    MatIconModule
  ],
  templateUrl: './startseite.component.html',
  styleUrl: './startseite.component.scss'
})
export class StartseiteComponent {
  turniername: string = '';

  onTurnierStarten() {
    console.log('Turniername: ', this.turniername);
  }
}
