import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PesagemService } from '../../services/pesagem.service';
import { Pesagem } from '../../models/pesagem.model';

// Componente para gerenciar a pesagem
@Component({
  selector: 'app-pesagem',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './pesagem.component.html',
})
// Classe do componente de pesagem
export class PesagemComponent implements OnInit {
  pesagens: Pesagem[] = [];
  novoPeso: number | null = null;

  constructor(private pesagemService: PesagemService) {}

  // Inicializa o componente e carrega as pesagens existentes
  ngOnInit(): void {
    this.carregarPesagens();
  }

  // Método para carregar as pesagens do backend
  carregarPesagens(): void {
    this.pesagemService.getPesagens().subscribe({
      next: (dados) => (this.pesagens = dados),
      error: (err) => console.error('Erro ao buscar pesagens:', err),
    });
  }

  // Método para salvar um novo peso
  salvarPeso(): void {
    if (!this.novoPeso) return;

    const pesagem: Pesagem = {
      data: new Date().toISOString(),
      peso: this.novoPeso,
    };

    this.pesagemService.registrarPesagem(pesagem).subscribe({
      next: () => {
        this.novoPeso = null;
        this.carregarPesagens();
      },
      error: (err) => console.error('Erro ao registrar pesagem:', err),
    });
  }

  // Método para deletar uma pesagem existente
  deletarPeso(id: number | undefined): void {
    if (!id) return;

    if (confirm('Tem certeza que deseja deletar esta pesagem?')) {
      this.pesagemService.deletarPesagem(id).subscribe({
        next: () => this.carregarPesagens(),
        error: (err) => console.error('Erro ao deletar pesagem:', err),
      });
    }
  }
}
