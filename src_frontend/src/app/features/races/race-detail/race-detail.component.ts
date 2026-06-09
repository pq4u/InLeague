import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';
import { RaceService } from '../../../core/services/race.service';
import { AuthService } from '../../../core/services/auth.service';
import { RaceResultService } from '../../../core/services/race-result.service';
import { Race } from '../../../core/models/race.model';
import { RaceTimePipe } from '../../../shared/pipes/race-time.pipe';

@Component({
  selector: 'app-race-detail',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatCardModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatTooltipModule,
    RaceTimePipe
  ],
  templateUrl: './race-detail.component.html',
  styleUrl: './race-detail.component.scss'
})
export class RaceDetailComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private raceService = inject(RaceService);
  private resultService = inject(RaceResultService);
  auth = inject(AuthService);

  race: Race | null = null;
  displayedColumns = ['pos', 'driver', 'kart', 'bestLap', 'totalTime', 'status'];

  ngOnInit(): void {
    if (this.auth.isAdmin()) {
        this.displayedColumns.push('actions');
    }

    const leagueId = this.route.snapshot.paramMap.get('leagueId');
    const raceId = this.route.snapshot.paramMap.get('raceId');

    if (leagueId && raceId) {
      this.loadRace(leagueId, raceId);
    }
  }

  loadRace(leagueId: string, raceId: string): void {
    this.raceService.getById(leagueId, raceId).subscribe({
      next: (data) => {
        this.race = data;
      },
      error: (err) => {
        console.error('Error loading race', err);
      }
    });
  }

  deleteResult(resultId: string): void {
    if (!confirm('Czy na pewno chcesz usunąć ten wynik?')) return;

    const leagueId = this.race!.leagueId;
    const raceId = this.race!.id;

    this.resultService.delete(raceId, resultId).subscribe({
      next: () => {
        this.loadRace(leagueId, raceId);
      },
      error: (err) => {
        console.error('Error deleting result', err);
      }
    });
  }
}
