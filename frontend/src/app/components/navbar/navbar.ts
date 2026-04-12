import { Component } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { CommonModule } from '@angular/common';
import { ConfirmationDialogComponent } from '../confirmation.dialog/confirmation.dialog';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, RouterLink, ConfirmationDialogComponent],
  templateUrl: './navbar.html',
  styleUrl: './navbar.css'
})
export class NavbarComponent {
  constructor(private authService: AuthService, private router: Router) {}

  isLoggedIn(): boolean {
    return this.authService.isLoggedIn();
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

    showLogoutDialog = false;

  onLogoutClick(): void {
    this.showLogoutDialog = true;
  }

  onConfirmLogout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
    this.showLogoutDialog = false;
  }

  onCancelLogout(): void {
    this.showLogoutDialog = false;
  }
}