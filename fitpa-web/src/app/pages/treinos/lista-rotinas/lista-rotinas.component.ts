import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-lista-rotinas',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './lista-rotinas.component.html'
})
export class ListaRotinasComponent implements OnInit {
  rotinas: any[] = []; 

  ngOnInit() {
    // this.rotinaService.listar().subscribe(...)
  }
}