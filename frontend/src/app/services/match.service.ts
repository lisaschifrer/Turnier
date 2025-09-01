import { Injectable } from "@angular/core";
import { environment } from "../../environments/environment";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";

@Injectable({providedIn: 'root'})
export class MatchService{
    private apiUrl = `${environment.apiUrl}/GroupMatches`;

    constructor(private http: HttpClient) {}

    generateMatches(turnierId: string): Observable<Match[]> {
        return this.http.post<Match[]>(`${this.apiUrl}/generate/${turnierId}`, {})
    }

    setWinner(matchId: string, winnerId:string): Observable<Match[]> {
        return this.http.put<Match[]>(`${this.apiUrl}/${matchId}/winner/${winnerId}`, {});
    }
}

export interface Match {
    id: string;
    groupId: string;
    teamA: string;
    teamB: string;
    winnerId?: string;
}