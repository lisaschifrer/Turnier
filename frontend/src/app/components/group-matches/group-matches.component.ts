import { Component, OnInit } from '@angular/core';
import { MatTabsModule } from '@angular/material/tabs';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatchService } from '../../services/match.service';
import { CommonModule } from '@angular/common';
import { PlacementService } from '../../services/placement.service';
import { Router } from '@angular/router'

@Component({
  selector: 'app-group-matches',
  imports: [
    MatTabsModule,
    MatTableModule,
    MatButtonModule,
    CommonModule
  ],
  templateUrl: './group-matches.component.html',
  styleUrl: './group-matches.component.scss'
})
export class GroupMatchesComponent implements OnInit{
  groups: any[] = [];
  groupNames: string[] = ['A', 'B', 'C', 'D', 'E', 'F', 'G', 'H'];
  turnierId!: string;

  constructor(private service: MatchService,
              private placementService: PlacementService,
              private router: Router
  ) {}

  ngOnInit(): void {
    // TODO: TurnierId dynamisch holen
    this.turnierId = localStorage.getItem('turnierId') ?? '';
    if(this.turnierId)
    {
      this.service.generateMatches(this.turnierId).subscribe(matches => {
      // Matches nach Gruppen sortieren
      const grouped: {[id: string]: any} = {};
      let index = 0;

      for (let match of matches) {
        if (!grouped[match.groupId]){
          if (!grouped[match.groupId]) {
          const groupName = this.groupNames[index] ?? '?';
          grouped[match.groupId] = { id: match.groupId, name: groupName, matches: [] }; 
        index++;
      };
        }
        grouped[match.groupId].matches.push(match);
      }
      this.groups = Object.values(grouped);
    });
    }
    else{
      console.error("Turnier nicht gefunden")
    }
  }

  setWinner(matchId: string, winnerId: string) {
    this.service.setWinner(matchId, winnerId).subscribe(updated => {
      console.log("Winner set:", updated);
    });
  }

  startKreuzspiele() {
  this.placementService.createAllBrackets(this.turnierId).subscribe({
    next: () => this.router.navigate([`/turnier/${this.turnierId}/kreuzspiele`]),
    error: (err) => {
      console.error('CreateAllBrackets error:', err);
      alert(err?.error?.message ?? 'Fehler beim Erstellen der Brackets – Details in der Konsole.');
    }
  });
}
}
