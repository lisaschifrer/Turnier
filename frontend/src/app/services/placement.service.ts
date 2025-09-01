import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class PlacementService {
  private apiUrl = `${environment.apiUrl}/placement`

  constructor(private http: HttpClient) { }

  createAllBrackets(turnierId: string): Observable<any[]> {
    return this.http.post<any[]>(`${this.apiUrl}/${turnierId}/create-all`, {});
  }
}
