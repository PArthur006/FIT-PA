import { Routes } from '@angular/router';
import { PesagemComponent } from './components/pesagem/pesagem.component';
import { LoginComponent } from './pages/login/login';

export const routes: Routes = [
    { path: 'pesagem', component: PesagemComponent },
    { path: 'login', component: LoginComponent },
    { path: '', redirectTo: '/login', pathMatch: 'full' } 
];
