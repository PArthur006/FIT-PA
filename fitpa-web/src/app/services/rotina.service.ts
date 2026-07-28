import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../src/environments/environment';

export interface RotinaExercicioDto {
  exercicioId: number;
  series: number;
  ordem: number;
}

export interface RotinaCreateDto {
  nome: string;
  exercicios: RotinaExercicioDto[];
}

export interface RotinaResponseDto {
  id: number;
  nome: string;
  exercicios: {
    exercicioId: number;
    nomeExercicio: string;
    grupoMuscular: string;
    series: number;
    ordem: number;
  }[];
}

@Injectable({
  providedIn: 'root',
})
export class RotinaService {
  private apiUrl = `${environment.apiUrl}/Rotinas`;

  constructor(private http: HttpClient) {}

  listarRotinas(): Observable<RotinaResponseDto> {
    return this.http.get<RotinaResponseDto>(this.apiUrl);
  }

  criarRotina(rotina: RotinaCreateDto): Observable<RotinaResponseDto> {
    return this.http.post<RotinaResponseDto>(this.apiUrl, rotina);
  }

  deletarRotina(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}