import { Routes } from '@angular/router';
import { PesagemComponent } from './components/pesagem/pesagem.component';
import { LoginComponent } from './pages/login/login';
import { RotinasComponent } from './pages/treinos/rotinas/rotinas.component';
import { NovoTreinoComponent } from './pages/treinos/novo-treino/novo-treino.component';
import { ListaRotinasComponent } from './pages/treinos/lista-rotinas/lista-rotinas.component';

export const routes: Routes = [
    { path: '', redirectTo: 'pesagem', pathMatch: 'full' },
    { path: 'login', component: LoginComponent },
    { path: 'pesagem', component: PesagemComponent },
    { path: 'nova-rotina', component: RotinasComponent },
    { path: 'novo-treino', component: NovoTreinoComponent },
    { path: 'lista-rotinas', component: ListaRotinasComponent },
];
