import { TestBed } from '@angular/core/testing';

import { Exercicio } from './exercicio';

describe('Exercicio', () => {
  let service: Exercicio;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(Exercicio);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
