import { Routes } from '@angular/router';
import { PesagemComponent } from './components/pesagem/pesagem.component';
import { LoginComponent } from './pages/login/login';
import { RotinasComponent } from './pages/treinos/rotinas/rotinas.component';

export const routes: Routes = [
    { path: 'pesagem', component: PesagemComponent },
    { path: 'login', component: LoginComponent },
    { path: 'nova-rotina', component: RotinasComponent },
    { path: '', redirectTo: '/login', pathMatch: 'full' } 
];
