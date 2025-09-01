import { Component } from '@angular/core';
import { MatTabsModule } from '@angular/material/tabs';
import { PlacementService } from '../../services/placement.service';
import { PlacementBracket } from '../../models/placement.models';
import { ActivatedRoute } from '@angular/router';
import { BracketInitialComponent } from '../bracket-initial/bracket-initial.component';
import { CommonModule } from '@angular/common';  

@Component({
  selector: 'app-kreuz-finalspiele',
  imports: [
    MatTabsModule,
    BracketInitialComponent,
    CommonModule
  ],
  templateUrl: './kreuz-finalspiele.component.html',
  styleUrl: './kreuz-finalspiele.component.scss'
})
export class KreuzFinalspieleComponent {

  turnierId!: string;

  // Map Rang -> Bracket
  brackets: { [rank: number]: PlacementBracket | null } = {
    1: null, 2: null, 3: null, 4: null, 5: null
  };

  loading = false;

  constructor(private service: PlacementService,
              private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id') ?? '';
    if (!id) {
    console.error('Keine TurnierId in der URL. Erwartet /turnier/:id/kreuzspiele');
    return;
    }
    this.turnierId = id;
    this.loadAllBrackets();
  }

  private loadAllBrackets(): void {
    this.loading = true;
    const ranks = [1,2,3,4,5];
    let loaded = 0;

    ranks.forEach(r => {
      this.service.getBracketByRank(this.turnierId, r).subscribe({
        next: b => { this.brackets[r] = b; if (++loaded === ranks.length) this.loading = false; },
        error: _ => { this.brackets[r] = null; if (++loaded === ranks.length) this.loading = false; }
      });
    });
  }
  bracketId(rank: number): string | null {
    return this.brackets[rank]?.id ?? null;
  }


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
