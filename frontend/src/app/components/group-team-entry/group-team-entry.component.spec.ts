import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GroupTeamEntryComponent } from './group-team-entry.component';

describe('GroupTeamEntryComponent', () => {
  let component: GroupTeamEntryComponent;
  let fixture: ComponentFixture<GroupTeamEntryComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GroupTeamEntryComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(GroupTeamEntryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
