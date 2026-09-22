import { inject, PLATFORM_ID } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { isPlatformBrowser } from '@angular/common';

export const guestGuard: CanActivateFn = (route, state) => {
  const router = inject(Router);
  const platformId = inject(PLATFORM_ID);

  if (isPlatformBrowser(platformId)) {
    const token = localStorage.getItem('jwt_token'); 
    
    if (token) {
      router.navigate(['/pesagem']);
      return false; // Bloqueia acesso ao login
    }
  }
  return true; // Deixa passar se não tiver logado
};