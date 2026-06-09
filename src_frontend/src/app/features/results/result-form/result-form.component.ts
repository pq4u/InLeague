import { Component, OnInit, inject } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { finalize } from 'rxjs';
import { RaceResultService } from '../../../core/services/race-result.service';
import { DriverService } from '../../../core/services/driver.service';
import { KartService } from '../../../core/services/kart.service';
import { Driver } from '../../../core/models/driver.model';
import { Kart } from '../../../core/models/kart.model';

@Component({
  selector: 'app-result-form',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    RouterModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatSelectModule,
    MatSnackBarModule
  ],
  templateUrl: './result-form.component.html',
  styleUrl: './result-form.component.scss'
})
export class ResultFormComponent implements OnInit {
  private fb = inject(FormBuilder);
  private resultService = inject(RaceResultService);
  private driverService = inject(DriverService);
  private kartService = inject(KartService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private snackBar = inject(MatSnackBar);

  drivers: Driver[] = [];
  karts: Kart[] = [];
  isLoading = false;
  leagueId: string | null = null;
  raceId: string | null = null;

  resultForm = this.fb.group({
    driverId: ['', [Validators.required]],
    kartId: ['', [Validators.required]],
    lapTime: this.fb.group({
      minutes: [0, [Validators.min(0)]],
      seconds: [0, [Validators.min(0), Validators.max(59)]],
      millis: [0, [Validators.min(0), Validators.max(999)]]
    }),
    totalTime: this.fb.group({
      minutes: [0, [Validators.min(0)]],
      seconds: [0, [Validators.min(0), Validators.max(59)]],
      millis: [0, [Validators.min(0), Validators.max(999)]]
    }),
    startingPosition: [1, [Validators.required, Validators.min(1)]],
    finishingPosition: [null as number | null, [Validators.min(1)]],
    lapsCompleted: [0, [Validators.required, Validators.min(0)]],
    status: ['Finished', [Validators.required]],
    notes: ['']
  });

  ngOnInit(): void {
    this.leagueId = this.route.snapshot.paramMap.get('leagueId');
    this.raceId = this.route.snapshot.paramMap.get('raceId');

    this.loadDrivers();
    this.loadKarts();
  }

  loadDrivers(): void {
    this.driverService.getAll().subscribe(data => this.drivers = data);
  }

  loadKarts(): void {
    this.kartService.getAll(true).subscribe(data => this.karts = data);
  }

  onSubmit(): void {
    if (this.resultForm.invalid) return;

    this.isLoading = true;
    const val = this.resultForm.value;

    const lapTimeMs = (val.lapTime?.minutes || 0) * 60000 + (val.lapTime?.seconds || 0) * 1000 + (val.lapTime?.millis || 0);
    const totalTimeMs = (val.totalTime?.minutes || 0) * 60000 + (val.totalTime?.seconds || 0) * 1000 + (val.totalTime?.millis || 0);

    const dto = {
      driverId: val.driverId,
      kartId: val.kartId,
      lapTimeMs,
      totalTimeMs,
      startingPosition: val.startingPosition,
      finishingPosition: val.finishingPosition,
      lapsCompleted: val.lapsCompleted,
      status: val.status,
      notes: val.notes
    };

    this.resultService.create(this.raceId!, dto as any).pipe(
      finalize(() => this.isLoading = false)
    ).subscribe({
      next: () => {
        this.snackBar.open('Wynik został pomyślnie dodany', 'Zamknij', { duration: 3000 });
        this.router.navigate(['/leagues', this.leagueId, 'races', this.raceId]);
      },
      error: (err) => {
        const errorMessage = err.error?.error || 'Nie udało się dodać wyniku';
        this.snackBar.open(errorMessage, 'Zamknij', { duration: 5000 });
      }
    });
  }

  onCancel(): void {
    this.router.navigate(['/leagues', this.leagueId, 'races', this.raceId]);
  }
}
