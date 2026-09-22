import { Component, OnInit, PLATFORM_ID, Inject } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ExercicioService, Exercicio } from '../../../services/exercicio.service';
import { TreinoService, TreinoCreateDto } from '../../../services/treino.service';

interface SerieUI { repeticoes: number; peso: number; }
interface ExercicioTreinoUI { exercicio: Exercicio; series: SerieUI[]; }

@Component({
  selector: 'app-novo-treino',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './novo-treino.component.html'
})
export class NovoTreinoComponent implements OnInit {
  catalogoCompleto: Exercicio[] = [];
  exerciciosFiltrados: Exercicio[] = [];
  termoPesquisa: string = '';
  grupoSelecionado: string = 'Todos';
  gruposMusculares: string[] = ['Todos', 'Peito', 'Costas', 'Pernas', 'Ombros', 'Bíceps', 'Tríceps', 'Core'];

  exerciciosExecutados: ExercicioTreinoUI[] = [];
  dataTreino: string = new Date().toISOString().split('T')[0];
  mensagemSucesso: string = '';

  // Controles de UX (Modais)
  isModalBuscaAberto: boolean = false;
  isModalEdicaoAberto: boolean = false;
  exercicioAtualEdicao: ExercicioTreinoUI | null = null;
  indexEdicaoAtual: number = -1;

  constructor(
    private exercicioService: ExercicioService,
    private treinoService: TreinoService,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {}

  ngOnInit() {
    if (isPlatformBrowser(this.platformId)) {
      this.exercicioService.listarTodos().subscribe(dados => {
        this.catalogoCompleto = dados;
        this.aplicarFiltros();
      });
    }
  }

  // --- Filtros e Busca ---
  aplicarFiltros() {
    this.exerciciosFiltrados = this.catalogoCompleto.filter(ex => {
      const batePesquisa = ex.nome.toLowerCase().includes(this.termoPesquisa.toLowerCase());
      const bateGrupo = this.grupoSelecionado === 'Todos' || ex.grupoMuscular === this.grupoSelecionado;
      return batePesquisa && bateGrupo;
    });
  }

  // --- Controle de Modais ---
  abrirModalBusca() {
    this.isModalBuscaAberto = true;
  }

  fecharModalBusca() {
    this.isModalBuscaAberto = false;
    this.termoPesquisa = '';
    this.aplicarFiltros();
  }

  selecionarExercicioDoCatalogo(exercicio: Exercicio) {
    const novoExercicio: ExercicioTreinoUI = {
      exercicio: exercicio,
      series: [{ repeticoes: 10, peso: 0 }]
    };
    this.exerciciosExecutados.push(novoExercicio);
    this.fecharModalBusca();
    
    // Já abre o modal de edição automaticamente para o usuário preencher a carga
    this.abrirModalEdicao(novoExercicio, this.exerciciosExecutados.length - 1);
  }

  abrirModalEdicao(exUI: ExercicioTreinoUI, index: number) {
    this.exercicioAtualEdicao = exUI;
    this.indexEdicaoAtual = index;
    this.isModalEdicaoAberto = true;
  }

  fecharModalEdicao() {
    this.isModalEdicaoAberto = false;
    this.exercicioAtualEdicao = null;
    this.indexEdicaoAtual = -1;
  }

  // --- Lógica de Edição de Séries (Dentro do Modal) ---
  adicionarSerie() {
    if (this.exercicioAtualEdicao) {
      this.exercicioAtualEdicao.series.push({ repeticoes: 10, peso: 0 });
    }
  }

  removerSerie(indexSerie: number) {
    if (this.exercicioAtualEdicao) {
      this.exercicioAtualEdicao.series.splice(indexSerie, 1);
    }
  }

  removerExercicioDoTreino(index: number) {
    this.exerciciosExecutados.splice(index, 1);
  }

  // --- Salvamento ---
  salvarTreino() {
    if (this.exerciciosExecutados.length === 0) return;

    const dto: TreinoCreateDto = {
      data: new Date(this.dataTreino).toISOString(),
      exerciciosExecutados: this.exerciciosExecutados.map(exUI => ({
        exercicioId: exUI.exercicio.id,
        series: exUI.series.map(s => ({ repeticoes: s.repeticoes, peso: s.peso }))
      }))
    };

    this.treinoService.registrarTreino(dto).subscribe({
      next: () => {
        this.mensagemSucesso = 'Treino Registrado!';
        this.exerciciosExecutados = [];
        setTimeout(() => this.mensagemSucesso = '', 3000);
      },
      error: (err) => console.error('Erro ao salvar treino', err)
    });
  }
}