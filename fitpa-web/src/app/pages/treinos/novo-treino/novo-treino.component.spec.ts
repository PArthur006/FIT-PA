import { ComponentFixture, TestBed } from '@angular/core/testing';

import { NovoTreino } from './novo-treino.component';

describe('NovoTreino', () => {
  let component: NovoTreino;
  let fixture: ComponentFixture<NovoTreino>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [NovoTreino]
    })
    .compileComponents();

    fixture = TestBed.createComponent(NovoTreino);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
