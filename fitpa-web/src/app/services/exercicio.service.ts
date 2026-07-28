import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../src/environments/environment';

export interface Exercicio {
  id: number;
  nome: string;
  grupoMuscular: string;
}

@Injectable({
  providedIn: 'root',
})
export class ExercicioService {
    private apiUrl = `${environment.apiUrl}/Exercicios`;

    constructor(private http: HttpClient) {}
    
    listarTodos(): Observable<Exercicio[]> {
      return this.http.get<Exercicio[]>(this.apiUrl);
    }
  }
