import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../../../core/auth/auth.service';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './login.html'
})

export class LoginComponent {
  email = '';
  password = '';
  error: string | null = null;
  loading = false;

  shopId: string | null = null;

  constructor(
    private auth: AuthService,
    private router: Router,
    private route: ActivatedRoute
  ) {
    this.shopId = this.route.snapshot.queryParamMap.get('shopId');
  }

  submit() {
    this.loading = true;
    this.error = null;

    this.auth.login({
      email: this.email,
      password: this.password
    })
    .pipe(finalize(() => (this.loading = false)))
    .subscribe({
      next: () => this.router.navigateByUrl(this.auth.defaultRoute()),
      error: () => this.error = 'Accesso non riuscito. Controlla le credenziali.'
    });
  }

  goRegister() {
    if (!this.shopId){
      // se manca shopId, register mostrerà l'errore guidato
      this.router.navigateByUrl('/register');
      return;
    }
    this.router.navigate(['/register'], {queryParams: { shopId: this.shopId } });
  }
}
