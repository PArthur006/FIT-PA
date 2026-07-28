import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ExercicioService, Exercicio } from '../../../services/exercicio.service';
import { RotinaService, RotinaCreateDto, RotinaExercicioDto } from '../../../services/rotina.service';

@Component({
  selector: 'app-rotinas',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './rotinas.component.html',
})
export class RotinasComponent implements OnInit {
  // Catálogo
  catalogoCompleto: Exercicio[] = [];
  exerciciosFiltrados: Exercicio[] = [];

  // Filtros
  termoPesquisa: string = '';
  grupoSelecionado: string = 'Todos';
  gruposMusculares: string[] = ['Todos', 'Peito', 'Costas', 'Pernas', 'Ombros', 'Bíceps', 'Tríceps', 'Core'];

  // Criação da Rotina
  nomeRotina: string = '';
  exercicioNaRotina: { exercicio: Exercicio, series: number }[] = [];

  mensagemSucesso: string = '';

  constructor(
    private exercicioService: ExercicioService,
    private rotinaService: RotinaService
  ) {}

  ngOnInit() {
    this.carregarCatalogo();
  }

  carregarCatalogo() {
    this.exercicioService.listarTodos().subscribe({
      next: (dados: Exercicio[]) => {
        this.catalogoCompleto = dados;
        this.aplicarFiltros();
      },
      error: (err) => console.error('Erro ao carregar catálogo:', err)
    });
  }

  aplicarFiltros() {
    this.exerciciosFiltrados = this.catalogoCompleto.filter(ex => {
      const batePesquisa = ex.nome.toLowerCase().includes(this.termoPesquisa.toLowerCase());
      const bateGrupo = this.grupoSelecionado === 'Todos' || ex.grupoMuscular === this.grupoSelecionado;
      return batePesquisa && bateGrupo;
    });
  }

  adicionarAoTreino(exercicio: Exercicio) {
    // Evita duplicidade simples
    if (!this.exercicioNaRotina.find(e => e.exercicio.id === exercicio.id)) {
      this.exercicioNaRotina.push({ exercicio: exercicio, series: 3 })
    }
  }

  removerDoTreino(index: number) {
    this.exercicioNaRotina.splice(index, 1);
  }

  mover(index: number, direcao: number) {
    const novoIndex = index + direcao;
    if (novoIndex >= 0 && novoIndex < this.exercicioNaRotina.length) {
      const temp = this.exercicioNaRotina[index];
      this.exercicioNaRotina[index] = this.exercicioNaRotina[novoIndex];
      this.exercicioNaRotina[novoIndex] = temp;
    }
  }

  salvarRotina() {
    if (!this.nomeRotina || this.exercicioNaRotina.length === 0) return;

    const dto: RotinaCreateDto = {
      nome: this.nomeRotina,
      exercicios: this.exercicioNaRotina.map((item, index) => ({
        exercicioId: item.exercicio.id,
        series: item.series,
        ordem: index + 1
      }))
    };

    this.rotinaService.criarRotina(dto).subscribe({
      next: () => {
        this.mensagemSucesso = 'Rotina criada com sucesso!';
        this.nomeRotina = '';
        this.exercicioNaRotina = [];
        setTimeout(() => this.mensagemSucesso = '', 3000);
      }
    });
  }
}