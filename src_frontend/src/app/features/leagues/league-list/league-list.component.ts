import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { finalize } from 'rxjs/operators';
import { LeagueService } from '../../../core/services/league.service';
import { AuthService } from '../../../core/services/auth.service';
import { League } from '../../../core/models/league.model';

@Component({
  selector: 'app-league-list',
  standalone: true,
  imports: [CommonModule, RouterModule, MatTableModule, MatButtonModule, MatIconModule, MatTooltipModule],
  templateUrl: './league-list.component.html',
  styleUrl: './league-list.component.scss'
})
export class LeagueListComponent implements OnInit {
  private leagueService = inject(LeagueService);
  auth = inject(AuthService);

  leagues = signal<League[]>([]);
  isLoading = signal(false);
  error = signal<string | null>(null);

  displayedColumns = ['name', 'startDate', 'endDate', 'raceCount', 'status', 'actions'];

  ngOnInit(): void {
    this.loadLeagues();
  }

  loadLeagues(): void {
    this.isLoading.set(true);
    this.error.set(null);

    this.leagueService.getAll()
      .pipe(finalize(() => this.isLoading.set(false)))
      .subscribe({
        next: data => {
          this.leagues.set(data);
        },
        error: err => {
          console.error('Error loading leagues', err);
          this.error.set('Nie udało się załadować lig');
        }
      });
  }

  deleteLeague(id: string): void {
    if (!confirm('Czy na pewno chcesz usunąć tę ligę?')) return;

    this.leagueService.delete(id).subscribe({
      next: () => this.loadLeagues(),
      error: (err) => {
        console.error('Error deleting league', err);
        alert('Nie udało się usunąć ligi');
      }
    });
  }
}
