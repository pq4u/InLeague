import { Component, OnInit, inject } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { finalize } from 'rxjs';
import { RaceService } from '../../../core/services/race.service';

@Component({
  selector: 'app-race-form',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    RouterModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatSnackBarModule
  ],
  templateUrl: './race-form.component.html',
  styleUrl: './race-form.component.scss'
})
export class RaceFormComponent implements OnInit {
  private fb = inject(FormBuilder);
  private raceService = inject(RaceService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private snackBar = inject(MatSnackBar);

  raceForm = this.fb.group({
    name: ['', [Validators.required]],
    location: [''],
    raceDate: ['', [Validators.required]],
    numberOfLaps: [10, [Validators.required, Validators.min(1)]],
    notes: ['']
  });

  isEdit = false;
  leagueId: string | null = null;
  raceId: string | null = null;
  isLoading = false;

  ngOnInit(): void {
    this.leagueId = this.route.snapshot.paramMap.get('leagueId');
    this.raceId = this.route.snapshot.paramMap.get('raceId');

    if (this.raceId) {
      this.isEdit = true;
      this.loadRace(this.leagueId!, this.raceId);
    }
  }

  loadRace(leagueId: string, raceId: string): void {
    this.isLoading = true;
    this.raceService.getById(leagueId, raceId).pipe(
      finalize(() => this.isLoading = false)
    ).subscribe({
      next: (race) => {
        this.raceForm.patchValue({
          name: race.name,
          location: race.location,
          raceDate: race.raceDate ? new Date(race.raceDate).toISOString().substring(0, 16) : '',
          numberOfLaps: race.numberOfLaps,
          notes: race.notes
        });
      },
      error: () => {
        this.snackBar.open('Błąd podczas ładowania danych wyścigu', 'Zamknij', { duration: 3000 });
      }
    });
  }

  onSubmit(): void {
    if (this.raceForm.invalid) return;

    this.isLoading = true;
    const formValue = this.raceForm.value;

    const request = this.isEdit
      ? this.raceService.update(this.leagueId!, this.raceId!, formValue as any)
      : this.raceService.create(this.leagueId!, formValue as any);

    request.pipe(
      finalize(() => this.isLoading = false)
    ).subscribe({
      next: (response) => {
        this.snackBar.open(`Wyścig został ${this.isEdit ? 'zaktualizowany' : 'utworzony'} pomyślnie`, 'Zamknij', { duration: 3000 });
        const targetRaceId = this.isEdit ? this.raceId : response.id;
        this.router.navigate(['/leagues', this.leagueId, 'races', targetRaceId]);
      },
      error: (err) => {
        const errorMessage = err.error?.error || 'Operacja nie powiodła się';
        this.snackBar.open(errorMessage, 'Zamknij', { duration: 5000 });
      }
    });
  }

  onCancel(): void {
    if (this.isEdit) {
      this.router.navigate(['/leagues', this.leagueId, 'races', this.raceId]);
    } else {
      this.router.navigate(['/leagues', this.leagueId]);
    }
  }
}
