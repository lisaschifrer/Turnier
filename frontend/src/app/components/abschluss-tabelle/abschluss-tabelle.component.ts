import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { ActivatedRoute } from '@angular/router';
import { PlacementService } from '../../services/placement.service';
import { MatCard } from '@angular/material/card';
import { MatCardTitle } from '@angular/material/card';

@Component({
  selector: 'app-abschluss-tabelle',
  standalone: true,
  imports: [CommonModule, MatTableModule, MatCard, MatCardTitle],
  templateUrl: './abschluss-tabelle.component.html',
  styleUrls: ['./abschluss-tabelle.component.scss']
})
export class AbschlussTabelleComponent implements OnInit {
  standings: any[] = [];
  displayedColumns = ['rank', 'team', 'points'];

  constructor(
    private placementService: PlacementService,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    const turnierId = this.route.snapshot.paramMap.get('id')!;
    this.placementService.getFinalStandings(turnierId).subscribe(data => {
      // Hier sortieren nach Platz (Backend könnte es schon tun)
      this.standings = data;
    });
  }
}
