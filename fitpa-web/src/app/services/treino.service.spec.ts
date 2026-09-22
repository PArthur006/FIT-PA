import { TestBed } from '@angular/core/testing';

import { Treino } from './treino';

describe('Treino', () => {
  let service: Treino;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(Treino);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
