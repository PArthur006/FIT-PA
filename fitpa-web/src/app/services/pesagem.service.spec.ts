import { TestBed } from '@angular/core/testing';

import { PesagemService } from './pesagem.service';

describe('PesagemService', () => {
  let service: PesagemService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(PesagemService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
