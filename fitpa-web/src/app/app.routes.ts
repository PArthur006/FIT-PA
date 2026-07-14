import { Routes } from '@angular/router';
import { PesagemComponent } from './components/pesagem/pesagem.component';

export const routes: Routes = [
    { path: 'pesagem', component: PesagemComponent },
    { path: '', redirectTo: '/pesagem', pathMatch: 'full' } 
];
