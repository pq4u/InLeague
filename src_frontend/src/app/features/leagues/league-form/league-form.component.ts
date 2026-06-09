import { Component, OnInit, inject, signal } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { finalize } from 'rxjs/operators';
import { LeagueService } from '../../../core/services/league.service';

@Component({
  selector: 'app-league-form',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    RouterModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatCheckboxModule,
    MatSnackBarModule
  ],
  templateUrl: './league-form.component.html',
  styleUrl: './league-form.component.scss'
})
export class LeagueFormComponent implements OnInit {
  private fb = inject(FormBuilder);
  private leagueService = inject(LeagueService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private snackBar = inject(MatSnackBar);

  leagueForm = this.fb.group({
    name: ['', [Validators.required]],
    description: [''],
    startDate: ['', [Validators.required]],
    endDate: [''],
    isActive: [true]
  });

  isEdit = false;
  leagueId: string | null = null;
  isLoading = signal(false);

  ngOnInit(): void {
    this.leagueId = this.route.snapshot.paramMap.get('id');
    if (this.leagueId) {
      this.isEdit = true;
      this.loadLeague(this.leagueId);
    }
  }

  loadLeague(id: string): void {
    this.isLoading.set(true);
    this.leagueService.getById(id)
      .pipe(finalize(() => this.isLoading.set(false)))
      .subscribe({
        next: (league) => {
          this.leagueForm.patchValue({
            name: league.name,
            description: league.description,
            startDate: league.startDate ? new Date(league.startDate).toISOString().substring(0, 10) : '',
            endDate: league.endDate ? new Date(league.endDate).toISOString().substring(0, 10) : '',
            isActive: league.isActive
          });
        },
        error: () => {
          this.snackBar.open('Błąd podczas ładowania danych ligi', 'Zamknij', { duration: 3000 });
        }
      });
  }

  onSubmit(): void {
    if (this.leagueForm.invalid) return;

    this.isLoading.set(true);
    const formValue = this.leagueForm.value;

    const request = this.isEdit
      ? this.leagueService.update(this.leagueId!, formValue as any)
      : this.leagueService.create(formValue as any);

    request.pipe(finalize(() => this.isLoading.set(false))).subscribe({
      next: () => {
        this.snackBar.open(`Liga została ${this.isEdit ? 'zaktualizowana' : 'utworzona'} pomyślnie`, 'Zamknij', { duration: 3000 });
        this.router.navigate(['/leagues']);
      },
      error: (err) => {
        const errorMessage = err.error?.error || 'Operacja nie powiodła się';
        this.snackBar.open(errorMessage, 'Zamknij', { duration: 5000 });
      }
    });
  }

  onCancel(): void {
    if (this.isEdit) {
      this.router.navigate(['/leagues', this.leagueId]);
    } else {
      this.router.navigate(['/leagues']);
    }
  }
}
