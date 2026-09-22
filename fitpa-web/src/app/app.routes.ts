import { Routes } from '@angular/router';
import { PesagemComponent } from './components/pesagem/pesagem.component';
import { LoginComponent } from './pages/login/login';
import { RotinasComponent } from './pages/treinos/rotinas/rotinas.component';
import { NovoTreinoComponent } from './pages/treinos/novo-treino/novo-treino.component';
import { ListaRotinasComponent } from './pages/treinos/lista-rotinas/lista-rotinas.component';

export const routes: Routes = [
    { path: 'pesagem', component: PesagemComponent },
    { path: 'login', component: LoginComponent },
    { path: 'nova-rotina', component: RotinasComponent },
    { path: '', redirectTo: '/login', pathMatch: 'full' },
    { path: 'novo-treino', component: NovoTreinoComponent },
    { path: 'lista-rotinas', loadComponent: () => import('./pages/treinos/lista-rotinas/lista-rotinas.component').then(m => m.ListaRotinasComponent) }
];
