import { Component, OnInit, inject } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { finalize } from 'rxjs';
import { KartService } from '../../../core/services/kart.service';

@Component({
  selector: 'app-kart-form',
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
  templateUrl: './kart-form.component.html',
  styleUrl: './kart-form.component.scss'
})
export class KartFormComponent implements OnInit {
  private fb = inject(FormBuilder);
  private kartService = inject(KartService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private snackBar = inject(MatSnackBar);

  kartForm = this.fb.group({
    number: ['', [Validators.required, Validators.maxLength(10)]],
    model: ['', [Validators.maxLength(50)]],
    category: ['', [Validators.maxLength(50)]],
    isActive: [true]
  });

  isEdit = false;
  id: string | null = null;
  isLoading = false;

  ngOnInit(): void {
    this.id = this.route.snapshot.paramMap.get('id');
    if (this.id) {
      this.isEdit = true;
      this.loadKart();
    }
  }

  loadKart(): void {
    this.isLoading = true;
    this.kartService.getById(this.id!).pipe(
      finalize(() => this.isLoading = false)
    ).subscribe({
      next: (kart) => this.kartForm.patchValue(kart),
      error: () => this.snackBar.open('Błąd podczas ładowania gokarta', 'Zamknij', { duration: 3000 })
    });
  }

  onSubmit(): void {
    if (this.kartForm.invalid) return;

    this.isLoading = true;
    const dto = this.kartForm.value;

    const request = this.isEdit
      ? this.kartService.update(this.id!, dto as any)
      : this.kartService.create(dto as any);

    request.pipe(
      finalize(() => this.isLoading = false)
    ).subscribe({
      next: () => {
        this.snackBar.open(`Gokart został ${this.isEdit ? 'zaktualizowany' : 'utworzony'} pomyślnie`, 'Zamknij', { duration: 3000 });
        this.router.navigate(['/admin/karts']);
      },
      error: (err) => {
        const msg = err.error?.error || `Błąd podczas ${this.isEdit ? 'aktualizowania' : 'tworzenia'} gokarta`;
        this.snackBar.open(msg, 'Zamknij', { duration: 5000 });
      }
    });
  }
}
