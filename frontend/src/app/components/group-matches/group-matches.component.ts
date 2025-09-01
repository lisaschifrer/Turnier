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

  constructor(private service: MatchService,
              private placementService: PlacementService,
              private router: Router
  ) {}

  ngOnInit(): void {
    // TODO: TurnierId dynamisch holen
    const turnierId = localStorage.getItem('turnierId');;
    if(turnierId)
    {
      this.service.generateMatches(turnierId).subscribe(matches => {
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
    //const turnierId = 'HIER-DEIN-TURNIER-ID'; // später dynamisch setzen
    //this.placementService.createAllBrackets(turnierId).subscribe({
      //next: () => {
        //console.log('Alle Brackets erzeugt');
    this.router.navigate(['/turnier/kreuzspiele']);
      //},
      //error: (err) => {
        //console.error('Fehler beim Erstellen der Brackets', err);
      //}
    //});
  }
}
