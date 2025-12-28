import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/auth/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './login.html'
})
export class LoginComponent {
  email = '';
  password = '';
  error: string | null = null;
  loading = false;

  constructor(
    private auth: AuthService,
    private router: Router
  ) {}

  submit() {
    this.loading = true;
    this.error = null;

    this.auth.login({
      email: this.email,
      password: this.password
    }).subscribe({
      next: () => {
        // 🔜 per ora vaiamo a /me
        this.router.navigateByUrl('/me');
      },
      error: () => {
        this.error = 'Credenziali non valide';
        this.loading = false;
      }
    });
  }
}
