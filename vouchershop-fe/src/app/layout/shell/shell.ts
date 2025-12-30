import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../core/auth/auth.service';

@Component({
  standalone: true,
  selector: 'app-shell',
  imports: [CommonModule, RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './shell.html',
  styleUrls: ['./shell.css'],
})
export class ShellComponent {
  constructor(public auth: AuthService, private router: Router) {}

  get isLoggedIn(): boolean {
    return !!this.auth.token;
  }

  get isAdmin(): boolean {
    return this.auth.hasRole('Admin');
  }

  get isUser(): boolean {
    return this.auth.hasRole('User');
  }

  logout(): void {
    this.auth.logout();
    this.router.navigateByUrl('/login');
  }
}
