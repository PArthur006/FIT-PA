import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Pesagem } from './pesagem.pages';

describe('Pesagem', () => {
  let component: Pesagem;
  let fixture: ComponentFixture<Pesagem>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Pesagem],
    }).compileComponents();

    fixture = TestBed.createComponent(Pesagem);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
