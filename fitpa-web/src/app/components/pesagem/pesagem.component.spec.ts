import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PesagemComponent } from './pesagem.component';

describe('Pesagem', () => {
  let component: PesagemComponent;
  let fixture: ComponentFixture<PesagemComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PesagemComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(PesagemComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
