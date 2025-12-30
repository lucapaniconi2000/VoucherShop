import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { finalize, switchMap } from 'rxjs/operators';
import { AuthService } from '../../../core/auth/auth.service';

function isGuid(value: string): boolean {
  return /^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[1-5][0-9a-fA-F]{3}-[89abAB][0-9a-fA-F]{3}-[0-9a-fA-F]{12}$/.test(value);
}

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './register.html'
})
export class RegisterComponent implements OnInit {
  shopId: string | null = null;

  email = '';
  password = '';
  confirmPassword = '';

  loading = false;
  error: string | null = null;

  get shopIdValid(): boolean {
    return !!this.shopId && isGuid(this.shopId);
  }

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private auth: AuthService
  ) {}

  ngOnInit(): void {
    this.shopId = this.route.snapshot.queryParamMap.get('shopId');
    if (!this.shopIdValid) {
      this.error = 'Link di registrazione non valido o mancante. Chiedi all’admin un link corretto.';
    }
  }

  submit(): void {
    this.error = null;

    if (!this.shopIdValid) {
      this.error = 'ShopId mancante o non valido.';
      return;
    }
    if (this.password !== this.confirmPassword) {
      this.error = 'Le password non coincidono.';
      return;
    }

    this.loading = true;

    this.auth.selfRegister({
      shopId: this.shopId!,
      email: this.email,
      password: this.password
    }).pipe(
      switchMap(() => this.auth.login({ email: this.email, password: this.password })),
      finalize(() => (this.loading = false))
    ).subscribe({
      next: () => this.router.navigateByUrl(this.auth.defaultRoute()),
      error: () => {
        this.error = 'Registrazione non riuscita. Controlla i dati e riprova.';
      }
    });
  }

  backToStore(): void {
    if (!this.shopId) return;
    this.router.navigate(['/store', this.shopId]);
  }
}
