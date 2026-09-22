import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ListaRotinas } from './lista-rotinas.component';

describe('ListaRotinas', () => {
  let component: ListaRotinas;
  let fixture: ComponentFixture<ListaRotinas>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ListaRotinas],
    }).compileComponents();

    fixture = TestBed.createComponent(ListaRotinas);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
