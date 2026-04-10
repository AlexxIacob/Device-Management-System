import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { DeviceService } from '../../services/device.service';
import { AuthService } from '../../services/auth.service';
import { Device } from '../../models/device.model';

@Component({
  selector: 'app-device-detail',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './device-detail.html',
  styleUrl: './device-detail.css'
})
export class DeviceDetailComponent implements OnInit {
  device: Device | null = null;
  errorMessage = '';
  currentUserId: string | null = null;

  constructor(
    private deviceService: DeviceService,
    private authService: AuthService,
    private route: ActivatedRoute,
    private router: Router,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.currentUserId = this.authService.getUserId();
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.deviceService.getById(id).subscribe({
        next: (device) => {
          this.device = device;
          this.cdr.detectChanges();
        },
        error: () => {
          this.errorMessage = 'Failed to load device.';
        }
      });
    }
  }

  isFree(): boolean {
    return !this.device?.assignedUserId;
  }

  isAssignedToCurrentUser(): boolean {
    return this.device?.assignedUserId === this.currentUserId;
  }

  onAssign(): void {
    if (!this.device) return;
    this.deviceService.assign(this.device.id).subscribe({
      next: () => {
        this.deviceService.getById(this.device!.id).subscribe({
          next: (device) => {
            this.device = device;
            this.cdr.detectChanges();
          }
        });
      },
      error: () => this.errorMessage = 'Failed to assign device.'
    });
  }

  onUnassign(): void {
    if (!this.device) return;
    this.deviceService.unassign(this.device.id).subscribe({
      next: () => {
        this.deviceService.getById(this.device!.id).subscribe({
          next: (device) => {
            this.device = device;
            this.cdr.detectChanges();
          }
        });
      },
      error: () => this.errorMessage = 'Failed to unassign device.'
    });
  }

  onBack(): void {
    this.router.navigate(['/home']);
  }
}