import { Injectable, Inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private apiUrl = 'http://localhost:5142/api/Auth';
  private isBrowser: boolean;

  constructor(
    private http: HttpClient,
    @Inject(PLATFORM_ID) platformId: Object,
  ) {
    this.isBrowser = isPlatformBrowser(platformId);
  }

  /*
   * Login
   * Envia as credenciais para a API e inicia a autenticação.
   */
  login(credenciais: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/login`, credenciais);
  }

  /*
   * Token JWT
   * Salva o token principal no localStorage do navegador.
   */
  salvarToken(token: string) {
    if (this.isBrowser) {
      localStorage.setItem('jwt_token', token);
    }
  }

  /*
   * Token JWT
   * Recupera o token principal salvo no localStorage.
   */
  obterToken(): string | null {
    if (this.isBrowser) {
      return localStorage.getItem('jwt_token');
    }
    return null;
  }

  /*
   * Sessão
   * Remove o token principal do localStorage ao sair.
   */
  sair() {
    if (this.isBrowser) {
      localStorage.removeItem('jwt_token');
    }
  }

  /*
   * Sessão
   * Verifica se existe token salvo para indicar autenticação ativa.
   */
  estaLogado(): boolean {
    return this.obterToken() !== null;
  }

  /*
   * Trust Token
   * Salva o token de confiança usado no fluxo de MFA.
   */
  salvarTrustToken(token: string) {
    if (this.isBrowser) {
      localStorage.setItem('mfa_trust_token', token);
    }
  }

  /*
   * Trust Token
   * Recupera o token de confiança salvo no localStorage.
   */
  obterTrustToken(): string | null {
    if (this.isBrowser) {
      return localStorage.getItem('mfa_trust_token');
    }
    return null;
  }
}
