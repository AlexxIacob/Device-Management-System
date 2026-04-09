import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './profile.html',
  styleUrl: './profile.css'
})
export class ProfileComponent {
  name = '';
  role = '';
  location = '';
  errorMessage = '';
  successMessage = '';

  constructor(private authService: AuthService, private router: Router) {}

  saveProfile(): void {
    if (!this.name || !this.role || !this.location) {
      this.errorMessage = 'All fields are required.';
      return;
    }

    this.authService.updateProfile({ name: this.name, role: this.role, location: this.location }).subscribe({
      next: () => {
            this.successMessage = 'Profile updated successfully.';
            setTimeout(() => {
              this.router.navigate(['/home']);
            }, 1000);
      },
      error: () => {
        this.errorMessage = 'Failed to update profile. Please try again.';
      }
    });
  }
}