import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PesagemService } from '../../services/pesagem.service';
import { Pesagem } from '../../models/pesagem.model';

@Component({
  selector: 'app-pesagem',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './pesagem.component.html',
})
export class PesagemComponent implements OnInit {
  /*
   * Estado da tela
   * Guarda os registros carregados e os valores do formulário.
   */
  pesagens: Pesagem[] = [];
  novoPeso: number | null = null;
  dataSelecionada: string = '';
  dataMaxima: string = '';
  pesagemEmEdicao: Pesagem | null = null;

  constructor(private pesagemService: PesagemService) {}

  /*
   * Inicialização
   * Carrega os registros e prepara a data padrão para o formulário.
   */
  ngOnInit(): void {
    this.carregarPesagens();
    this.dataSelecionada = this.obterDataAtualFormatada();
    this.dataMaxima = this.obterDataAtualFormatada();
  }

  /*
   * Utilitário de data
   * Formata a data atual no padrão YYYY-MM-DD para uso no input.
   */
  obterDataAtualFormatada(): string {
    const agora = new Date();
    agora.setMinutes(agora.getMinutes() - agora.getTimezoneOffset());
    return agora.toISOString().slice(0, 10);
  }

  private extrairDataParaInput(data: string): string {
    return data.slice(0, 10);
  }

  carregarPesagens(): void {
    this.pesagemService.getPesagens().subscribe({
      next: (dados) => (this.pesagens = dados),
      error: (err) => console.error('Erro ao buscar pesagens:', err),
    });
  }

  /*
   * Salvamento
   * Valida duplicidade e data futura antes de criar ou atualizar um registro.
   */
  salvarPeso(): void {
    if (!this.novoPeso || !this.dataSelecionada) return;

    const dataJaExiste = this.pesagens.some((p) => {
      const mesmaData = p.data === this.dataSelecionada;
      const isMesmoRegistro = this.pesagemEmEdicao && this.pesagemEmEdicao.id === p.id;

      return mesmaData && !isMesmoRegistro;
    });

    if (this.dataSelecionada > this.obterDataAtualFormatada()) {
      alert('Você não pode registrar um peso para uma data futura.');
      return;
    }

    if (dataJaExiste) {
      alert('Você já registrou um peso para este dia. Edite o registro existente na lista abaixo.');
      return;
    }

    if (this.pesagemEmEdicao && this.pesagemEmEdicao.id) {
      const pesagemAtualizada: Pesagem = {
        id: this.pesagemEmEdicao.id,
        data: this.dataSelecionada,
        peso: this.novoPeso,
      };

      this.pesagemService.atualizarPesagem(pesagemAtualizada.id!, pesagemAtualizada).subscribe({
        next: () => this.cancelarEdicao(),
        error: (err) => {
          console.error('Erro ao atualizar pesagem:', err);
          alert('Erro ao atualizar. Verifique se a data já existe.');
        },
      });
    } else {
      const novaPesagem: Pesagem = {
        data: this.dataSelecionada,
        peso: this.novoPeso,
      };

      this.pesagemService.registrarPesagem(novaPesagem).subscribe({
        next: () => this.cancelarEdicao(),
        error: (err) => {
          console.error('Erro ao registrar pesagem:', err);
          alert('Erro ao registrar. Verifique se a data já existe.');
        },
      });
    }
  }

  /*
   * Edição
   * Carrega os valores do item selecionado no formulário.
   */
  editarPeso(p: Pesagem): void {
    this.pesagemEmEdicao = p;
    this.novoPeso = p.peso;

    this.dataSelecionada = this.extrairDataParaInput(p.data);
  }

  /*
   * Exclusão
   * Remove um registro após confirmação do usuário.
   */
  deletarPeso(id: number | undefined): void {
    if (!id) return;

    if (confirm('Tem certeza que deseja excluir este registro?')) {
      this.pesagemService.deletarPesagem(id).subscribe({
        next: () => {
          if (this.pesagemEmEdicao?.id === id) this.cancelarEdicao();
          this.carregarPesagens();
        },
        error: (err) => console.error('Erro ao excluir pesagem:', err),
      });
    }
  }

  /*
   * Cancelamento
   * Limpa o estado de edição e restaura os valores padrão do formulário.
   */
  cancelarEdicao(): void {
    this.pesagemEmEdicao = null;
    this.novoPeso = null;
    this.dataSelecionada = this.obterDataAtualFormatada();
    this.carregarPesagens();
  }
}
