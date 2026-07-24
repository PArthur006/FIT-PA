import { Component, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './login.html',
})
export class LoginComponent {
  credenciais = { username: '', password: '', mfaCode: '' };
  precisaMfa = false;
  erro = '';
  carregando = false;

  constructor(
    private authService: AuthService, 
    private router: Router,
    private cdr: ChangeDetectorRef // Injetamos o forçador de renderização
  ) {}

  entrar() {
    if (!this.credenciais.username || !this.credenciais.password) {
      this.erro = 'Preencha usuário e senha.';
      return;
    }

    this.erro = '';
    this.carregando = true;
    this.cdr.detectChanges(); // Obriga o HTML a mostrar o "Processando..."

    this.authService.login(this.credenciais).subscribe({
      next: (res) => {
        this.authService.salvarToken(res.token);
        this.carregando = false;
        this.router.navigate(['/pesagem']); 
      },
      error: (err) => {
        console.log("1. Erro interceptado do C#", err);
        this.carregando = false;
        
        let erroObj = err.error;
        if (typeof erroObj === 'string') {
          try { erroObj = JSON.parse(erroObj); } 
          catch (e) { console.error("Falha ao ler JSON", e); }
        }

        console.log("2. Pacote extraído:", erroObj);

        if (err.status === 401 && erroObj?.requiresMfa) {
          console.log("3. MFA acionado! O campo vai aparecer na tela.");
          this.precisaMfa = true;
          this.erro = 'MFA exigido. Insira o código do seu Authenticator.';
        } else {
          console.log("3. Bloqueio padrão (senha errada ou código MFA inválido).");
          this.erro = erroObj?.mensagem || 'Usuário, senha ou código inválidos.';
          this.precisaMfa = false; 
        }

        // O GOLPE FINAL: Obriga o HTML a se redesenhar com as novas variáveis
        this.cdr.detectChanges(); 
      }
    });
  }
}