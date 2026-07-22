import { Injectable, Inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = 'http://localhost:5142/api/Auth';
  private isBrowser: boolean;

  constructor(
    private http: HttpClient,
    @Inject(PLATFORM_ID) platformId: Object
  ) {
    this.isBrowser = isPlatformBrowser(platformId);
  }

  // Dispara o usuário e senha para a API
  login(credenciais: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/login`, credenciais);
  }

  // Guarda o Token JWT no localStorage
  salvarToken(token: string) {
    if (this.isBrowser) {
      localStorage.setItem('jwt_token', token);
    }
  }

  // Pega o Token JWT do localStorage
  obterToken(): string | null {
    if (this.isBrowser) {
      return localStorage.getItem('jwt_token');
    }
    return null;
  }

  // Remove o Token JWT do localStorage
  sair() {
    if (this.isBrowser) {
      localStorage.removeItem('jwt_token');
    }
  }

  // Verifica se o usuário está logado
  estaLogado(): boolean {
    return this.obterToken() !== null;
  }
}