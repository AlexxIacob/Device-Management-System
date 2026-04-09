import { Component } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent {
  email = '';
  password = '';
  confirmPassword = '';
  errorMessage = '';

  constructor(private authService: AuthService, private router: Router) {}

  register(): void {
    if (!this.email || !this.password || !this.confirmPassword) {
      this.errorMessage = 'All fields are required.';
      return;
    }

    if (this.password !== this.confirmPassword) {
      this.errorMessage = 'Passwords do not match.';
      return;
    }

    if (this.password.length < 7) {
      this.errorMessage = 'Password must be at least 7 characters.';
      return;
    }

    this.authService.register(this.email, this.password).subscribe({
      next: () => {
        this.authService.login(this.email, this.password).subscribe({
          next: () => {
            this.router.navigate(['/profile']);
          },
          error: () => {
            this.router.navigate(['/login']);
          }
        });
      },
      error: (err) => {
        this.errorMessage = err.error || 'Registration failed. Please try again.';
      }
    });
  }
}