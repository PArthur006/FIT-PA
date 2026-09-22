import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface TreinoSerieDto {
  repeticoes: number;
  peso: number;
}

export interface TreinoExercicioDto {
  exercicioId: number;
  series: TreinoSerieDto[];
}

export interface TreinoCreateDto {
  data: string;
  exerciciosExecutados: TreinoExercicioDto[];
}

@Injectable({ providedIn: 'root' })
export class TreinoService {
  private apiUrl = `${environment.apiUrl}/Treinos`;

  constructor(private http: HttpClient) {}

  registrarTreino(treino: TreinoCreateDto): Observable<any> {
    return this.http.post(this.apiUrl, treino);
  }
}