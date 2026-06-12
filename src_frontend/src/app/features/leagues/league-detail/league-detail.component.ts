import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDividerModule } from '@angular/material/divider';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';
import { LeagueService } from '../../../core/services/league.service';
import { AuthService } from '../../../core/services/auth.service';
import { League } from '../../../core/models/league.model';

@Component({
  selector: 'app-league-detail',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatCardModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatDividerModule,
    MatProgressSpinnerModule,
    MatTooltipModule
  ],
  templateUrl: './league-detail.component.html',
  styleUrl: './league-detail.component.scss'
})
export class LeagueDetailComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private leagueService = inject(LeagueService);
  auth = inject(AuthService);

  league: League | null = null;
  displayedColumns = ['name', 'date', 'location', 'actions'];

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.loadLeague(id);
    }
  }

  loadLeague(id: string): void {
    this.leagueService.getById(id).subscribe({
      next: (data) => {
        this.league = data;
      },
      error: (err) => {
        console.error('Error loading league', err);
      }
    });
  }
}
