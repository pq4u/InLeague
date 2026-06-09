import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { DriverService } from '../../../core/services/driver.service';
import { AuthService } from '../../../core/services/auth.service';
import { Driver } from '../../../core/models/driver.model';

@Component({
  selector: 'app-driver-list',
  standalone: true,
  imports: [CommonModule, RouterModule, MatTableModule, MatButtonModule, MatIconModule, MatSnackBarModule],
  templateUrl: './driver-list.component.html',
  styleUrl: './driver-list.component.scss'
})
export class DriverListComponent implements OnInit {
  private driverService = inject(DriverService);
  private snackBar = inject(MatSnackBar);
  auth = inject(AuthService);

  drivers: Driver[] = [];
  displayedColumns = ['name', 'number', 'dob', 'actions'];

  ngOnInit(): void {
    this.loadDrivers();
  }

  loadDrivers(): void {
    this.driverService.getAll().subscribe({
      next: (data) => this.drivers = data,
      error: () => this.snackBar.open('Błąd podczas ładowania kierowców', 'Zamknij', { duration: 3000 })
    });
  }

  deleteDriver(id: string): void {
    if (!confirm('Czy na pewno chcesz usunąć tego kierowcę?')) return;

    this.driverService.delete(id).subscribe({
      next: () => {
        this.snackBar.open('Kierowca został usunięty', 'Zamknij', { duration: 3000 });
        this.loadDrivers();
      },
      error: (err) => {
        const msg = err.status === 409 ? 'Nie można usunąć kierowcy z istniejącymi wynikami' : 'Błąd podczas usuwania kierowcy';
        this.snackBar.open(msg, 'Zamknij', { duration: 5000 });
      }
    });
  }
}
