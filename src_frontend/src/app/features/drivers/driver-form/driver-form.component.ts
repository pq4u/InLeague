import { Component, OnInit, inject } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { finalize } from 'rxjs';
import { DriverService } from '../../../core/services/driver.service';

@Component({
  selector: 'app-driver-form',
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
  templateUrl: './driver-form.component.html',
  styleUrl: './driver-form.component.scss'
})
export class DriverFormComponent implements OnInit {
  private fb = inject(FormBuilder);
  private driverService = inject(DriverService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private snackBar = inject(MatSnackBar);

  driverForm = this.fb.group({
    firstName: ['', [Validators.required]],
    lastName: ['', [Validators.required]],
    racingNumber: [''],
    dateOfBirth: ['']
  });

  isEdit = false;
  driverId: string | null = null;
  isLoading = false;

  ngOnInit(): void {
    this.driverId = this.route.snapshot.paramMap.get('id');
    if (this.driverId) {
      this.isEdit = true;
      this.loadDriver(this.driverId);
    }
  }

  loadDriver(id: string): void {
    this.isLoading = true;
    this.driverService.getById(id).pipe(
      finalize(() => this.isLoading = false)
    ).subscribe({
      next: (driver) => {
        this.driverForm.patchValue({
          firstName: driver.firstName,
          lastName: driver.lastName,
          racingNumber: driver.racingNumber,
          dateOfBirth: driver.dateOfBirth ? new Date(driver.dateOfBirth).toISOString().substring(0, 10) : ''
        });
      },
      error: () => {
        this.snackBar.open('Błąd podczas ładowania danych kierowcy', 'Zamknij', { duration: 3000 });
      }
    });
  }

  onSubmit(): void {
    if (this.driverForm.invalid) return;

    this.isLoading = true;
    const formValue = this.driverForm.value;

    const request = this.isEdit
      ? this.driverService.update(this.driverId!, formValue as any)
      : this.driverService.create(formValue as any);

    request.pipe(
      finalize(() => this.isLoading = false)
    ).subscribe({
      next: () => {
        this.snackBar.open(`Kierowca został ${this.isEdit ? 'zaktualizowany' : 'utworzony'} pomyślnie`, 'Zamknij', { duration: 3000 });
        this.router.navigate(['/admin/drivers']);
      },
      error: (err) => {
        const errorMessage = err.error?.error || 'Operacja nie powiodła się';
        this.snackBar.open(errorMessage, 'Zamknij', { duration: 5000 });
      }
    });
  }
}
