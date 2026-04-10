import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { DeviceService } from '../../services/device.service';
import { AuthService } from '../../services/auth.service';
import { Device } from '../../models/device.model';
import { DeviceTableComponent } from '../../components/device-table/device-table';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-my-devices',
  standalone: true,
  imports: [CommonModule, DeviceTableComponent, FormsModule],
  templateUrl: './my-devices.html',
  styleUrl: './my-devices.css'
})
export class MyDevicesComponent implements OnInit {
  devices: Device[] = [];
  filteredDevices: Device[] = [];
  currentUserId: string | null = null;
  errorMessage = '';
  searchQuery = '';

  constructor(
    private deviceService: DeviceService,
    private authService: AuthService,
    private router: Router,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.currentUserId = this.authService.getUserId();
    this.loadDevices();
  }

  loadDevices(): void {
    this.deviceService.getAll().subscribe({
      next: (devices) => {
        this.devices = devices.filter(d => d.assignedUserId === this.currentUserId);
        this.filteredDevices = this.devices;
        this.cdr.detectChanges();
      },
      error: () => {
        this.errorMessage = 'Failed to load devices.';
      }
    });
  }

  onSearch(query: string): void {
    this.searchQuery = query;
    if (!query.trim()) {
      this.filteredDevices = this.devices;
      return;
    }
    const q = query.toLowerCase();
    this.filteredDevices = this.devices.filter(d =>
      d.name.toLowerCase().includes(q) ||
      d.manufacturer.toLowerCase().includes(q) ||
      d.processor.toLowerCase().includes(q) ||
      d.ram.toString().includes(q)
    );
  }

  onViewDevice(id: string): void {
    this.router.navigate(['/devices', id]);
  }

  onUnassignDevice(id: string): void {
    this.deviceService.unassign(id).subscribe({
      next: () => {
        this.loadDevices();
        this.cdr.detectChanges();
      },
      error: () => this.errorMessage = 'Failed to unassign device.'
    });
  }
}