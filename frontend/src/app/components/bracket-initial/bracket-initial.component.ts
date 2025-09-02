import { Component, OnInit, OnChanges, Input, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDividerModule } from '@angular/material/divider';
import { PlacementService } from '../../services/placement.service';
import { PlacementBracket, Team, FinalMatch } from '../../models/placement.models';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-bracket-initial',
  standalone: true,
  imports: [
    CommonModule, FormsModule,
    MatFormFieldModule, MatSelectModule, MatButtonModule, MatCardModule, MatDividerModule, RouterModule
  ],
  templateUrl: './bracket-initial.component.html',
  styleUrls: ['./bracket-initial.component.scss']
})
export class BracketInitialComponent implements OnInit, OnChanges {

  @Input() bracketId!: string;   // kommt vom Tab/Parent

  bracket?: PlacementBracket;
  teams: Team[] = [];
  matches: FinalMatch[] = [];
  saving = false;
  matchesAll: FinalMatch[] = [];   // Runde 1 + 2
  matchesR2: FinalMatch[] = [];
  standings: { teamId: string, name: string, wins: number, losses: number }[] = [];
  matchesR3: FinalMatch[] = [];
  finalPlacements: { place: number, teamName: string }[] = [];

  selected: { [k: number]: { teamAId?: string; teamBId?: string } } = {
    1: {}, 2: {}, 3: {}, 4: {}
  };

  constructor(private placement: PlacementService) {}

  ngOnInit(): void {
    if (this.bracketId) this.loadBracket();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['bracketId'] && this.bracketId) {
      this.resetSelections();
      this.loadBracket();
    }
  }

  private resetSelections() {
    this.selected = { 1: {}, 2: {}, 3: {}, 4: {} };
    this.matches = [];
    this.teams = [];
    this.bracket = undefined;
  }

  private loadBracket(): void {
    this.placement.getBracket(this.bracketId).subscribe(b => {
      this.bracket = b;
      const participants = (b.participants ?? []).sort((a, z) => a.seed - z.seed);
      this.teams = participants.map(p => p.team);
      this.refreshMatches();
    });
  }

  refreshMatches(): void {
    this.refreshAll();
  }

  isTeamDisabled(teamId: string, slot: 'A'|'B', gameIndex: number): boolean {
    for (const idx of [1,2,3,4]) {
      if (idx === gameIndex) continue;
      const s = this.selected[idx];
      if (s?.teamAId === teamId || s?.teamBId === teamId) return true;
    }
    const s = this.selected[gameIndex];
    if (slot === 'A' && s?.teamBId === teamId) return true;
    if (slot === 'B' && s?.teamAId === teamId) return true;
    return false;
  }

  canSave(): boolean {
    for (const i of [1,2,3,4]) {
      const s = this.selected[i];
      if (!s.teamAId || !s.teamBId) return false;
      if (s.teamAId === s.teamBId) return false;
    }
    return true;
  }

  saveMatches(): void {
    if (!this.canSave()) return;
    const pairs = [1,2,3,4].map(i => ({
      teamAId: this.selected[i].teamAId!, teamBId: this.selected[i].teamBId!
    }));
    this.saving = true;
    this.placement.createInitialMatches(this.bracketId, pairs).subscribe({
      next: ms => { this.matches = ms; this.saving = false; },
      error: _ => { this.saving = false; }
    });
  }

  private refreshAll(): void {
  this.placement.getAllMatches(this.bracketId).subscribe(ms => {
    console.log('Matches vom Server:', ms); // ← sieh dir die IDs an
    this.matchesAll = [...ms].sort((a,b)=> a.roundNumber-b.roundNumber || a.indexInRound-b.indexInRound);
    this.matches   = this.matchesAll.filter(m=>m.roundNumber === 1);
    this.matchesR2 = this.matchesAll.filter(m=>m.roundNumber === 2);
    this.matchesR3 = this.matchesAll.filter(m => m.roundNumber === 3);
    this.computeStandings();

    const r3Done = this.matchesR3.length === 4 && this.matchesR3.every(m => !!m.winnerId);
    if (r3Done) {
      this.loadPlacements();
    } else {
      this.finalPlacements = [];
    }
  });
}

  private loadPlacements(): void {
  this.placement.getPlacements(this.bracketId).subscribe(list => {
    this.finalPlacements = list.map(x => ({ place: x.place, teamName: x.teamName }));
  });
}

private computeStandings(): void {
  const map = new Map<string, { teamId: string, name: string, wins: number, losses: number }>();
  const nameOf = (id: string) => this.teams.find(t => t.id === id)?.name ?? id;

  for (const m of this.matchesAll) {
    if (!m.teamAId || !m.teamBId) continue; // nicht gesetzt
    const a = m.teamAId, b = m.teamBId;

    if (!map.has(a)) map.set(a, { teamId: a, name: nameOf(a), wins: 0, losses: 0 });
    if (!map.has(b)) map.set(b, { teamId: b, name: nameOf(b), wins: 0, losses: 0 });

    if (m.winnerId) {
      if (m.winnerId === a) { map.get(a)!.wins++; map.get(b)!.losses++; }
      else if (m.winnerId === b) { map.get(b)!.wins++; map.get(a)!.losses++; }
    }
  }

  this.standings = Array.from(map.values())
    .sort((x, y) => y.wins - x.wins || x.losses - y.losses || x.name.localeCompare(y.name));
}

  setWinner(matchId: string | undefined, winnerId: string | undefined): void {
  if (!matchId || !winnerId) { console.warn('setWinner: ungültige Werte', { matchId, winnerId }); return; }
  console.log('setWinner call', { matchId, winnerId });
  this.placement.setWinner(matchId, winnerId).subscribe(_ => this.refreshAll());
}

  teamName(id: string): string {
    const t = this.teams.find(x => x.id === id);
    return t ? t.name : id;
  }
}
