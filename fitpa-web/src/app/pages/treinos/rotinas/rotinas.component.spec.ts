import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Rotinas } from './rotinas.component';

describe('Rotinas', () => {
  let component: Rotinas;
  let fixture: ComponentFixture<Rotinas>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Rotinas],
    }).compileComponents();

    fixture = TestBed.createComponent(Rotinas);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
