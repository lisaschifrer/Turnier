import { Routes } from '@angular/router';
import { StartseiteComponent } from './components/startseite/startseite.component';
import { GroupTeamEntryComponent } from './components/group-team-entry/group-team-entry.component';

export const routes: Routes = [
  { path: '', component: StartseiteComponent },
  { path: 'turnier/teams', component: GroupTeamEntryComponent },
];