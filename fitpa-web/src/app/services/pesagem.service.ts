import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Pesagem } from '../models/pesagem.model';

@Injectable({
  providedIn: 'root',
})
export class PesagemService {
  private apiUrl = 'http://localhost:5142/api/pesagem';

  constructor(private http: HttpClient) {}

  getPesagens(): Observable<Pesagem[]> {
    return this.http.get<Pesagem[]>(this.apiUrl);
  }

  registrarPesagem(pesagem: Pesagem): Observable<Pesagem> {
    return this.http.post<Pesagem>(this.apiUrl, pesagem);
  }

  atualizarPesagem(id: number, pesagem: Pesagem): Observable<Pesagem> {
    return this.http.put<Pesagem>(`${this.apiUrl}/${id}`, pesagem);
  }

  deletarPesagem(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
