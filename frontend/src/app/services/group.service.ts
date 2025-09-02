import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { environment } from "../../environments/environment";

@Injectable({ providedIn: 'root'})
export class GroupService{
    constructor(private http: HttpClient){}

    getGroups(): Observable<any[]> {
        return this.http.get<any[]>(environment.apiUrl + '/Group');
    }

    getStandings(groupId: string) {
  return this.http.get<Array<{teamId:string; teamName:string; points:number; rank:number}>>(
    `${environment.apiUrl}/Group/${groupId}/standings`
  );
}
}