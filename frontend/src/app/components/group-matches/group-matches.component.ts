import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { MatTabsModule } from '@angular/material/tabs';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';

import { MatchService } from '../../services/match.service';
import { PlacementService } from '../../services/placement.service';
import { GroupService } from '../../services/group.service';

@Component({
  selector: 'app-group-matches',
  standalone: true,
  imports: [CommonModule, MatTabsModule, MatTableModule, MatButtonModule],
  templateUrl: './group-matches.component.html',
  styleUrls: ['./group-matches.component.scss'] // <-- plural + Array
})
export class GroupMatchesComponent implements OnInit {
  groups: Array<{ id: string; name: string; matches: any[] }> = [];
  groupNames: string[] = ['A', 'B', 'C', 'D', 'E', 'F', 'G', 'H'];
  turnierId!: string;

  standingsByGroup: Record<string, Array<{ teamId: string; teamName: string; points: number; rank: number }>> = {};
  displayedColumnsMatches = ['teams', 'actions'];
  displayedColumnsStandings = ['rank', 'team', 'points'];

  constructor(
    private service: MatchService,
    private placementService: PlacementService,
    private router: Router,
    private groupService: GroupService
  ) {}

  private rngFromSeed(seed: string) {
  let h = 2166136261 >>> 0;
  for (let i = 0; i < seed.length; i++) {
    h ^= seed.charCodeAt(i);
    h = Math.imul(h, 16777619);
  }
  return () => {
    // mulberry32
    h += 0x6D2B79F5;
    let t = Math.imul(h ^ (h >>> 15), 1 | h);
    t ^= t + Math.imul(t ^ (t >>> 7), 61 | t);
    return ((t ^ (t >>> 14)) >>> 0) / 4294967296;
  };
}

private shuffleInPlace<T>(arr: T[], rnd: () => number) {
  for (let i = arr.length - 1; i > 0; i--) {
    const j = Math.floor(rnd() * (i + 1));
    [arr[i], arr[j]] = [arr[j], arr[i]];
  }
}

/** Mischt Matches so, dass möglichst keine Mannschaft zweimal hintereinander dran ist */
private mixMatchesForDisplay(matches: any[], seed: string): any[] {
  const rnd = this.rngFromSeed(seed);
  const pool = [...matches];
  this.shuffleInPlace(pool, rnd); // Grundshuffle

  const out: any[] = [];
  let lastTeams = new Set<string>();

  while (pool.length) {
    // finde Match, das keine Überschneidung mit den zuletzt gespielten Teams hat
    let idx = pool.findIndex(m =>
      !lastTeams.has(m.teamAId) && !lastTeams.has(m.teamBId)
    );

    if (idx === -1) {
      // Notlösung: nimm irgendeines (minimale Überschneidung), damit wir vorankommen
      idx = 0;
      // optional: suche eines mit max. 1 Überschneidung
      const candidate = pool.findIndex(m =>
        !(lastTeams.has(m.teamAId) && lastTeams.has(m.teamBId))
      );
      if (candidate !== -1) idx = candidate;
    }

    const m = pool.splice(idx, 1)[0];
    out.push(m);
    lastTeams = new Set<string>([m.teamAId, m.teamBId]);
  }

  return out;
}

  private bumpStandingsOptimistic(groupId: string, winnerTeamId: string) {
  const list = this.standingsByGroup[groupId];
  if (!list || !list.length) return;

  // Punkte +1 beim Gewinner
  const updated = list.map(s =>
    s.teamId === winnerTeamId ? { ...s, points: s.points + 1 } : s
  );

  // Dense-Ranking neu setzen: nach Punkten desc, Name als Tiebreaker
  updated.sort((a, b) =>
    b.points - a.points || a.teamName.localeCompare(b.teamName)
  );
  let rank = 0;
  let prevPts: number | null = null;
  for (const s of updated) {
    if (prevPts === null || s.points !== prevPts) {
      rank = (updated.findIndex(x => x === s) + 1);
      prevPts = s.points;
    }
    s.rank = rank;
  }

  this.standingsByGroup[groupId] = updated;
  // neue Referenz -> Change Detection
  this.standingsByGroup = { ...this.standingsByGroup };
}

/** Betroffene Gruppe zu einem Match finden */
private findGroupIdByMatchId(matchId: string): string | null {
  const grp = this.groups.find(g => g.matches?.some((m: any) => m.id === matchId));
  return grp ? grp.id : null;
}

  ngOnInit(): void {
    this.turnierId = localStorage.getItem('turnierId') ?? '';
    if (!this.turnierId) {
      console.error('Turnier nicht gefunden');
      return;
    }
    this.loadGroupsAndMatches();
  }

  /** Lädt alle Matches, gruppiert sie nach GroupId und lädt danach die Standings je Gruppe */
  private loadGroupsAndMatches(): void {
    this.service.generateMatches(this.turnierId).subscribe({
      next: (matches: any[]) => {
        const grouped: Record<string, { id: string; name: string; matches: any[] }> = {};
        let nextNameIndex = 0;

        // Falls Matches bereits einen GroupName haben, kannst du den direkt verwenden.
        for (const match of matches) {
          if (!grouped[match.groupId]) {
            const groupName = match.groupName ?? this.groupNames[nextNameIndex] ?? '?';
            grouped[match.groupId] = { id: match.groupId, name: groupName, matches: [] };
            nextNameIndex++;
          }
          grouped[match.groupId].matches.push(match);
        }

        // Option: Matches in jeder Gruppe sortieren (z. B. nach Teamnamen)
        for (const g of Object.values(grouped)) {
          const seed = `${this.turnierId}|${g.name}`;
          g.matches = this.mixMatchesForDisplay(g.matches, seed);
}

        this.groups = Object.values(grouped);

        // Nach dem Setzen der Gruppen: Standings laden
        this.loadAllStandings();
      },
      error: err => console.error('generateMatches error', err)
    });
  }

  /** Standings aller geladenen Gruppen laden */
  private loadAllStandings(): void {
    for (const g of this.groups) {
      this.loadStandingsForGroup(g.id);
    }
  }

  /** Standings einer spezifischen Gruppe aktualisieren */
  private loadStandingsForGroup(groupId: string): void {
    this.groupService.getStandings(groupId).subscribe({
      next: list => {
        this.standingsByGroup[groupId] = list;
        // neue Referenz -> UI-Update sicherstellen
        this.standingsByGroup = { ...this.standingsByGroup };
      },
      error: err => console.error('getStandings error', err)
    });
  }

  /** Sieger setzen -> betroffene Gruppe neu laden (Matches + Standings) */
  setWinner(matchId: string, winnerId: string) {
    this.service.setWinner(matchId, winnerId).subscribe({
      next: _ => {
        // finde die betroffene Gruppe
        const grp = this.groups.find(gr => gr.matches?.some(m => m.id === matchId));
        if (grp) {
          // nur diese Gruppe neu laden (schonend)
          this.service.generateMatches(this.turnierId).subscribe(allMatches => {
            const groupMatches = (allMatches as any[]).filter(m => m.groupId === grp.id);
            grp.matches = [...groupMatches]; // neue Referenz
            this.loadStandingsForGroup(grp.id);
          });
        } else {
          // Fallback: alles neu laden
          this.loadGroupsAndMatches();
        }
      },
      error: err => console.error('setWinner error', err)
    });
  }

  onWinnerClick(match: any, winnerId: string | null | undefined) {
  if (!match?.id || !winnerId) return;

  // ✅ Optimistisches UI-Update → sofort sichtbar
  match.winnerId = winnerId;

  const groupId = match.groupId ?? this.findGroupIdByMatchId(match.id);
  if (groupId) {
    this.bumpStandingsOptimistic(groupId, winnerId);
  }

  // API-Call
  this.service.setWinner(match.id, winnerId).subscribe({
    next: updated => {
      // Backend gibt das Match mit winnerId zurück → wir mergen nur diese Zeile
      Object.assign(match, updated);

      // Standings aus dem Backend nachladen (Quelle der Wahrheit)
      this.loadStandingsForGroup(groupId);
    },
    error: err => {
      console.error(err);
      // optional: zurücksetzen, falls Server-Fehler
      match.winnerId = null;
      if (groupId) {
        // komplette Standings neu vom Server holen
        this.loadStandingsForGroup(groupId);
      }
    }
  });
}

  winnerName(match: { winnerId?: string; teamAId?: string; teamBId?: string; teamAName: string; teamBName: string }): string {
    if (!match?.winnerId) return '';
    return match.winnerId === match.teamAId ? match.teamAName : match.teamBName;
  }

  startKreuzspiele() {
    this.placementService.createAllBrackets(this.turnierId).subscribe({
      next: () => this.router.navigate([`/turnier/${this.turnierId}/kreuzspiele`]),
      error: err => {
        console.error('CreateAllBrackets error:', err);
        alert(err?.error?.message ?? 'Fehler beim Erstellen der Brackets – Details in der Konsole.');
      }
    });
  }
}
