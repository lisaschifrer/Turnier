import { Routes } from '@angular/router';
import { StartseiteComponent } from './components/startseite/startseite.component';
import { GroupTeamEntryComponent } from './components/group-team-entry/group-team-entry.component';
import { GroupMatchesComponent } from './components/group-matches/group-matches.component';
import { KreuzFinalspieleComponent } from './components/kreuz-finalspiele/kreuz-finalspiele.component';

export const routes: Routes = [
  { path: '', component: StartseiteComponent },
  { path: 'turnier/teams', component: GroupTeamEntryComponent },
  { path: 'turnier/gruppenspiele', component: GroupMatchesComponent},
  { path: 'turnier/:id/kreuzspiele', component: KreuzFinalspieleComponent}
];