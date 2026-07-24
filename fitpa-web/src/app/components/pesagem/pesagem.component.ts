import { Component, OnInit, Inject, PLATFORM_ID, ChangeDetectorRef } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
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
  pesagens: Pesagem[] = [];
  novoPeso: number | null = null;
  dataSelecionada: string = '';
  dataMaxima: string = '';
  pesagemEmEdicao: Pesagem | null = null;
  private isBrowser: boolean;

  constructor(
    private pesagemService: PesagemService,
    @Inject(PLATFORM_ID) platformId: Object,
    private cdr: ChangeDetectorRef,
  ) {
    this.isBrowser = isPlatformBrowser(platformId);
  }

  /*
   * Inicialização
   * Configura as datas da tela e carrega as pesagens apenas no navegador.
   */
  ngOnInit(): void {
    this.dataSelecionada = this.obterDataAtualFormatada();
    this.dataMaxima = this.obterDataAtualFormatada();

    if (this.isBrowser) {
      this.carregarPesagens();
    }
  }

  /*
   * Data atual
   * Retorna a data do dia no formato compatível com input date.
   */
  obterDataAtualFormatada(): string {
    const agora = new Date();
    agora.setMinutes(agora.getMinutes() - agora.getTimezoneOffset());
    return agora.toISOString().slice(0, 10);
  }

  /*
   * Data para edição
   * Extrai a parte da data usada pelo campo de formulário.
   */
  private extrairDataParaInput(data: string): string {
    return data.slice(0, 10);
  }

  /*
   * Carregamento
   * Busca as pesagens da API e atualiza a lista exibida.
   */
  carregarPesagens(): void {
    if (!this.isBrowser) return;

    this.pesagemService.getPesagens().subscribe({
      next: (dados) => {
        this.pesagens = dados;
        this.cdr.detectChanges();
      },
      error: (err) => console.error('Erro ao buscar pesagens:', err),
    });
  }

  /*
   * Salvamento
   * Valida data, evita duplicidade e decide entre criar ou atualizar.
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
   * Carrega o item selecionado nos campos do formulário.
   */
  editarPeso(p: Pesagem): void {
    this.pesagemEmEdicao = p;
    this.novoPeso = p.peso;
    this.dataSelecionada = this.extrairDataParaInput(p.data);
    this.cdr.detectChanges();
  }

  /*
   * Exclusão
   * Remove uma pesagem após confirmação do usuário.
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
   * Limpa o estado de edição e restaura os campos da tela.
   */
  cancelarEdicao(): void {
    this.pesagemEmEdicao = null;
    this.novoPeso = null;
    this.dataSelecionada = this.obterDataAtualFormatada();
    this.cdr.detectChanges();
    this.carregarPesagens();
  }
}
