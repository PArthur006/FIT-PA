import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth/auth.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const token = authService.obterToken();

  /*
   * Authorization
   * Adiciona o token JWT ao header quando o usuário está autenticado.
   */
  if (token) {
    const authReq = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`,
      },
    });
    return next(authReq);
  }

  /*
   * Requisição original
   * Deixa a chamada seguir sem alteração quando não há token salvo.
   */
  return next(req);
};
