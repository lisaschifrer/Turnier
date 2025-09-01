import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BracketInitialComponent } from './bracket-initial.component';

describe('BracketInitialComponent', () => {
  let component: BracketInitialComponent;
  let fixture: ComponentFixture<BracketInitialComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BracketInitialComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(BracketInitialComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
