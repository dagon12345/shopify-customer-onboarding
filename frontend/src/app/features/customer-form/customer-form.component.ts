import { CommonModule } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { CustomerApiService } from '../../core/services/customer-api.service';
import { CreateCustomerResponse, ProblemDetails } from '../../core/models/customer.model';

type SubmitState = 'idle' | 'submitting' | 'success' | 'error';

@Component({
  selector: 'app-customer-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './customer-form.component.html',
  styleUrl: './customer-form.component.scss',
})
export class CustomerFormComponent {
  private readonly fb = inject(FormBuilder);
  private readonly customerApi = inject(CustomerApiService);

  protected readonly state = signal<SubmitState>('idle');
  protected readonly errorMessage = signal<string | null>(null);
  protected readonly result = signal<CreateCustomerResponse | null>(null);

  protected readonly form = this.fb.nonNullable.group({
    firstName: ['', [Validators.required, Validators.maxLength(100)]],
    lastName: ['', [Validators.required, Validators.maxLength(100)]],
    email: ['', [Validators.required, Validators.email, Validators.maxLength(254)]],
    phone: ['', [Validators.pattern(/^\+?[0-9()\-.\s]{7,20}$/)]],
    acceptsMarketing: [false],
    note: ['', [Validators.maxLength(1000)]],
  });

  protected get f() {
    return this.form.controls;
  }

  protected submit(): void {
    if (this.form.invalid || this.state() === 'submitting') {
      this.form.markAllAsTouched();
      return;
    }

    const raw = this.form.getRawValue();
    this.state.set('submitting');
    this.errorMessage.set(null);

    this.customerApi
      .createCustomer({
        firstName: raw.firstName.trim(),
        lastName: raw.lastName.trim(),
        email: raw.email.trim(),
        phone: raw.phone.trim() || null,
        acceptsMarketing: raw.acceptsMarketing,
        note: raw.note.trim() || null,
      })
      .subscribe({
        next: (response) => {
          this.result.set(response);
          this.state.set('success');
        },
        error: (err: HttpErrorResponse) => {
          this.errorMessage.set(this.extractErrorMessage(err));
          this.state.set('error');
        },
      });
  }

  protected resetForm(): void {
    this.form.reset({ acceptsMarketing: false });
    this.state.set('idle');
    this.result.set(null);
    this.errorMessage.set(null);
  }

  private extractErrorMessage(err: HttpErrorResponse): string {
    const problem = err.error as ProblemDetails | undefined;

    if (problem?.errors) {
      return Object.values(problem.errors).flat().join(' ');
    }

    if (problem?.detail) {
      return problem.detail;
    }

    if (err.status === 0) {
      return 'Could not reach the server. Please check your connection and try again.';
    }

    return 'Something went wrong while submitting the form. Please try again.';
  }
}
