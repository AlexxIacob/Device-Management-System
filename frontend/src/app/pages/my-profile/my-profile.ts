import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-my-profile',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './my-profile.html',
  styleUrl: './my-profile.css'
})
export class MyProfileComponent implements OnInit {
  name = '';
  role = '';
  location = '';
  email = '';
  errorMessage = '';
  successMessage = '';

  constructor(
    private authService: AuthService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.authService.getProfile().subscribe({
      next: (user) => {
        this.name = user.name;
        this.role = user.role;
        this.location = user.location;
        this.email = user.email;
        this.cdr.detectChanges();
      },
      error: () => {
        this.errorMessage = 'Failed to load profile.';
        this.cdr.detectChanges();
      }
    });
  }

  saveProfile(): void {
    if (!this.name || !this.role || !this.location) {
      this.errorMessage = 'All fields are required.';
      return;
    }

    this.authService.updateProfile({ name: this.name, role: this.role, location: this.location }).subscribe({
      next: () => {
        this.successMessage = 'Profile updated successfully.';
        this.errorMessage = '';
        this.cdr.detectChanges();
      },
      error: () => {
        this.errorMessage = 'Failed to update profile.';
        this.successMessage = '';
        this.cdr.detectChanges();
      }
    });
  }
}