import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { environment } from "../../environments/environment";

@Injectable({providedIn: 'root'})
export class TeamService{
    constructor(private http: HttpClient) {}

    addTeam(groupId: string, name: string): Observable<any> {
        return this.http.post(environment.apiUrl + '/Group/' + groupId + '/teams', {name});
    }
}