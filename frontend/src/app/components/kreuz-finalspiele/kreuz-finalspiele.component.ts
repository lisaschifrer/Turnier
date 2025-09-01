import { Component } from '@angular/core';
import { MatTabsModule } from '@angular/material/tabs';
import { PlacementService } from '../../services/placement.service';

@Component({
  selector: 'app-kreuz-finalspiele',
  imports: [
    MatTabsModule
  ],
  templateUrl: './kreuz-finalspiele.component.html',
  styleUrl: './kreuz-finalspiele.component.scss'
})
export class KreuzFinalspieleComponent {
  constructor(private service: PlacementService) {}

  startBrackets(){
    const turnierId = '...'  // id noch laden
    this.service.createAllBrackets(turnierId).subscribe({
      next: (brackets) => {
        console.log('Alle Brackets erstellt:', brackets);
      },
      error: (err) => {
        console.error('Fehler beim erstellen der Brackets', err);
      }
    });
  }
}
