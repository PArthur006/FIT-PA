import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './login.html',
  styleUrls: ['./login.css'],
})
export class LoginComponent {
  credenciais = { username: '', password: '', mfaCode: '' };
  precisaMfa = false;
  erro = '';
  carregando = false;

  constructor(private authService: AuthService, private router: Router) {}

  entrar() {
    if (!this.credenciais.username || !this.credenciais.password) {
      this.erro = 'Preencha usuário e senha.';
      return;
    }

    this.erro = '';
    this.carregando = true;

    this.authService.login(this.credenciais).subscribe({
      next: (res) => {
        this.authService.salvarToken(res.token);
        this.router.navigate(['/pesagem']);
      },
      error: (err) => {
        this.carregando = false;

        if (err.status === 401 && err.error?.requiresMfa) {
          this.precisaMfa = true;
          this.erro = 'MFA exigido. Insira o código do seu Authenticator.';
        } else {
          this.erro = err.error?.mensagem || err.error || 'Credenciais inválidas.';
          this.precisaMfa = false;
        }
      }
    });
  }
}