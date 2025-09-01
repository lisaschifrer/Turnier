import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { PlacementBracket, FinalMatch } from '../models/placement.models';


@Injectable({
  providedIn: 'root'
})
export class PlacementService {
  private apiUrl = `${environment.apiUrl}/placement`

  constructor(private http: HttpClient) { }

  createAllBrackets(turnierId: string): Observable<PlacementBracket[]> {
    return this.http.post<PlacementBracket[]>(`${this.apiUrl}/${turnierId}/create-all`, {});
  }

  getBracket(bracketId: string): Observable<PlacementBracket>{
    return this.http.get<PlacementBracket>(`${this.apiUrl}/${bracketId}`);
  }

  createInitialMatches(bracketId: string, pairs: {teamAId: string, teamBId: string}[]): Observable<FinalMatch[]> {
    return this.http.post<FinalMatch[]>(`${this.apiUrl}/${bracketId}/matches/initial`, pairs);
  }

  getInitialMatches(bracketId: string): Observable<FinalMatch[]> {
    return this.http.get<FinalMatch[]>(`${this.apiUrl}/${bracketId}/matches/initial`, {});
  }

  setWinner(matchId: string, winnerId: string): Observable<FinalMatch> {
    return this.http.post<FinalMatch>(`${this.apiUrl}/matches/${matchId}/winner/${winnerId}`, {});
}

  getBracketByRank(turnierId: string, rank: number) {
  return this.http.get<PlacementBracket>(`${this.apiUrl}/${turnierId}/by-rank/${rank}`);
}

  getAllMatches(bracketId: string){
    return this.http.get<FinalMatch[]>(`${this.apiUrl}/${bracketId}/matches`);
  }

  getPlacements(bracketId: string) {
  return this.http.get<{ place: number, teamId: string, teamName: string }[]>(
    `${this.apiUrl}/${bracketId}/placements`
  );
}
}
