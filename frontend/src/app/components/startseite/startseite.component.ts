import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatError } from '@angular/material/form-field';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { MatSnackBar, MatSnackBarConfig } from '@angular/material/snack-bar';
import { Router } from '@angular/router'


@Component({
  selector: 'app-startseite',
  imports: [
    FormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatError,
    MatButtonModule,
    MatIconModule,
  ],
  templateUrl: './startseite.component.html',
  styleUrl: './startseite.component.scss'
})
export class StartseiteComponent {
  turniername: string = '';

  constructor(private httpClient: HttpClient, 
              private snackBar: MatSnackBar,
              private router: Router){}

  onTurnierStarten() {
    const payload = {
      name: this.turniername
    };

    this.httpClient.post(environment.apiUrl + '/Turnier', payload)
      .subscribe({
        next: (response) => {
          this.snackBar.open('Turnier erfolgreich gespeichert 🎉', 'Schließen', {
            duration: 3000,
            panelClass: ['snackbar-success']
          });
        },
        error: (err) => {
          console.error('Fehler beim Speichern des Turniers:', err);
          alert('Fehler beim Starten des Turniers.');
        }
      });

      this.router.navigate(['/turnier/teams']); 
  }
}
