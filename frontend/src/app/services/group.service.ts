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
}