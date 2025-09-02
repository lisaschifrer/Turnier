import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AbschlussTabelleComponent } from './abschluss-tabelle.component';

describe('AbschlussTabelleComponent', () => {
  let component: AbschlussTabelleComponent;
  let fixture: ComponentFixture<AbschlussTabelleComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AbschlussTabelleComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AbschlussTabelleComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
