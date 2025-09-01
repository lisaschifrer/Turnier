import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { GroupService } from '../../services/group.service';
import { TeamService } from '../../services/team.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-group-team-entry',
  imports: [
    FormsModule,
    CommonModule,
    MatCardModule,
    MatTableModule,
    MatInputModule,
    MatButtonModule,
    MatFormFieldModule
  ],
  templateUrl: './group-team-entry.component.html',
  styleUrl: './group-team-entry.component.scss'
})
export class GroupTeamEntryComponent implements OnInit{

    groups: any[] = [];
    teamInputs: { [groupId: string] : string[]} = {};
    displayedColumns: string[] = ['team', 'actions'];

    constructor(
        private groupService: GroupService, 
        private teamService: TeamService,
        private router: Router){ }

   ngOnInit(): void {
     this.groupService.getGroups().subscribe(groups =>{
      console.log(groups);
      this.groups = groups;
      for (let group of groups){
        this.teamInputs[group.id] = Array(5).fill('');
      }
     });
   }

   saveTeam(groupId: string, index: number): void {
    const name = this.teamInputs[groupId] [index];
    if(!name.trim()) return;

    this.teamService.addTeam(groupId, name).subscribe(() => {
      console.log(`Team" ${name}" gespeichert`);
    });
   }

   // Button um weiter in die Gruppenphase zu kommen
   startGroupPhase(): void{
    if (this.groups.length > 0) {
    const turnierId = this.groups[0].turnierId; // 👈 alle Gruppen haben dieselbe TurnierId
    localStorage.setItem('turnierId', turnierId); // optional speichern
    this.router.navigate(['/turnier/gruppenspiele']);
  } else {
    console.error("Keine Gruppen gefunden, TurnierId fehlt!");
  }
   }
}
