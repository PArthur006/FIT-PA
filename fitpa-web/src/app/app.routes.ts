import { Routes } from '@angular/router';
import { PesagemComponent } from './components/pesagem/pesagem.component';
import { LoginComponent } from './pages/login/login';
import { RotinasComponent } from './pages/treinos/rotinas/rotinas.component';
import { NovoTreinoComponent } from './pages/treinos/novo-treino/novo-treino.component';
import { ListaRotinasComponent } from './pages/treinos/lista-rotinas/lista-rotinas.component';

import { authGuard } from './guards/auth-guard';
import { guestGuard } from './guards/guest-guard';

export const routes: Routes = [
    { path: '', redirectTo: 'pesagem', pathMatch: 'full' },

    { path: 'login', component: LoginComponent, canActivate: [guestGuard] },

    { path: 'pesagem', component: PesagemComponent, canActivate: [authGuard] },
    { path: 'nova-rotina', component: RotinasComponent, canActivate: [authGuard] },
    { path: 'novo-treino', component: NovoTreinoComponent, canActivate: [authGuard] },
    { path: 'lista-rotinas', component: ListaRotinasComponent, canActivate: [authGuard] },
];
