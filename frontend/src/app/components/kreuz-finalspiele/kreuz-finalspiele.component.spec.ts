import { ComponentFixture, TestBed } from '@angular/core/testing';

import { KreuzFinalspieleComponent } from './kreuz-finalspiele.component';

describe('KreuzFinalspieleComponent', () => {
  let component: KreuzFinalspieleComponent;
  let fixture: ComponentFixture<KreuzFinalspieleComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [KreuzFinalspieleComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(KreuzFinalspieleComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
