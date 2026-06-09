import { Component, OnInit, inject } from '@angular/core';
import { RouterModule } from '@angular/router';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { KartService } from '../../../core/services/kart.service';
import { AuthService } from '../../../core/services/auth.service';
import { Kart } from '../../../core/models/kart.model';

@Component({
  selector: 'app-kart-list',
  standalone: true,
  imports: [RouterModule, MatTableModule, MatButtonModule, MatIconModule, MatSnackBarModule],
  templateUrl: './kart-list.component.html',
  styleUrl: './kart-list.component.scss'
})
export class KartListComponent implements OnInit {
  private kartService = inject(KartService);
  private snackBar = inject(MatSnackBar);
  auth = inject(AuthService);

  karts: Kart[] = [];
  displayedColumns = ['number', 'model', 'category', 'status', 'actions'];

  ngOnInit(): void {
    this.loadKarts();
  }

  loadKarts(): void {
    this.kartService.getAll().subscribe({
      next: (data) => this.karts = data,
      error: () => this.snackBar.open('Błąd podczas ładowania gokartów', 'Zamknij', { duration: 3000 })
    });
  }

  deleteKart(id: string): void {
    if (!confirm('Czy na pewno chcesz usunąć ten gokart?')) return;

    this.kartService.delete(id).subscribe({
      next: () => {
        this.snackBar.open('Gokart został usunięty', 'Zamknij', { duration: 3000 });
        this.loadKarts();
      },
      error: (err) => {
        const msg = err.status === 409 ? 'Nie można usunąć gokarta z istniejącymi wynikami' : 'Błąd podczas usuwania gokarta';
        this.snackBar.open(msg, 'Zamknij', { duration: 5000 });
      }
    });
  }
}
