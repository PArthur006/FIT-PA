import { Component, OnInit, PLATFORM_ID, Inject } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ExercicioService, Exercicio } from '../../../services/exercicio.service';
import { TreinoService, TreinoCreateDto } from '../../../services/treino.service';

// Tipos locais apenas para a tela (UI)
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
  dataTreino: string = new Date().toISOString().split('T')[0]; // Padrão: Hoje (YYYY-MM-DD)
  mensagemSucesso: string = '';

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

  aplicarFiltros() {
    this.exerciciosFiltrados = this.catalogoCompleto.filter(ex => {
      const batePesquisa = ex.nome.toLowerCase().includes(this.termoPesquisa.toLowerCase());
      const bateGrupo = this.grupoSelecionado === 'Todos' || ex.grupoMuscular === this.grupoSelecionado;
      return batePesquisa && bateGrupo;
    });
  }

  adicionarExercicio(exercicio: Exercicio) {
    this.exerciciosExecutados.push({
      exercicio: exercicio,
      series: [{ repeticoes: 10, peso: 0 }] // Ao adicionar, já cria 1 série em branco
    });
  }

  removerExercicio(index: number) {
    this.exerciciosExecutados.splice(index, 1);
  }

  adicionarSerie(indexExercicio: number) {
    this.exerciciosExecutados[indexExercicio].series.push({ repeticoes: 10, peso: 0 });
  }

  removerSerie(indexExercicio: number, indexSerie: number) {
    this.exerciciosExecutados[indexExercicio].series.splice(indexSerie, 1);
  }

  salvarTreino() {
    if (this.exerciciosExecutados.length === 0) return;

    // Converte o modelo visual para o DTO que a API exige
    const dto: TreinoCreateDto = {
      data: new Date(this.dataTreino).toISOString(), // Converte para o padrão C# (UTC)
      exerciciosExecutados: this.exerciciosExecutados.map(exUI => ({
        exercicioId: exUI.exercicio.id,
        series: exUI.series.map(s => ({ repeticoes: s.repeticoes, peso: s.peso }))
      }))
    };

    this.treinoService.registrarTreino(dto).subscribe({
      next: () => {
        this.mensagemSucesso = 'Treino salvo e registrado com sucesso!';
        this.exerciciosExecutados = [];
        setTimeout(() => this.mensagemSucesso = '', 3000);
      },
      error: (err) => console.error('Erro ao salvar treino', err)
    });
  }
}