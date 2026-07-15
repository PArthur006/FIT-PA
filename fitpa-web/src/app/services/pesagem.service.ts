import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Pesagem } from '../models/pesagem.model';

@Injectable({
  providedIn: 'root',
})
export class PesagemService {
  /*
   * Endpoint base da API
   * Centraliza a URL usada em todas as operações de pesagem.
   */
  private apiUrl = 'http://localhost:5142/api/pesagem';

  constructor(private http: HttpClient) {}

  /*
   * Leitura
   * Busca a lista completa de pesagens cadastradas.
   */
  getPesagens(): Observable<Pesagem[]> {
    return this.http.get<Pesagem[]>(this.apiUrl);
  }

  /*
   * Criação
   * Envia um novo registro de pesagem para a API.
   */
  registrarPesagem(pesagem: Pesagem): Observable<Pesagem> {
    return this.http.post<Pesagem>(this.apiUrl, pesagem);
  }

  /*
   * Atualização
   * Substitui os dados de uma pesagem existente pelo ID.
   */
  atualizarPesagem(id: number, pesagem: Pesagem): Observable<Pesagem> {
    return this.http.put<Pesagem>(`${this.apiUrl}/${id}`, pesagem);
  }

  /*
   * Exclusão
   * Remove uma pesagem pelo identificador informado.
   */
  deletarPesagem(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
