import { inject, PLATFORM_ID } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { isPlatformBrowser } from '@angular/common';

export const authGuard: CanActivateFn = (route, state) => {
  const router = inject(Router);
  const platformId = inject(PLATFORM_ID);

  if (isPlatformBrowser(platformId)) {
    const token = localStorage.getItem('jwt_token');
    
    if (!token) {
      router.navigate(['/login']);
      return false; // Bloqueia acesso ao app
    }
  }
  return true; // Deixa passar se tiver logado
};