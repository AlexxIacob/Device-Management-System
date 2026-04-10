import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { DeviceService } from '../../services/device.service';
import { CreateDevice } from '../../models/device.model';
import { DeviceFormFieldsComponent } from '../device.form.fields/device.form.fields';

@Component({
  selector: 'app-device-form',
  standalone: true,
  imports: [CommonModule, DeviceFormFieldsComponent],
  templateUrl: './device.form.html',
  styleUrl: './device.form.css'
})
export class DeviceFormComponent implements OnInit {
  isEditMode = false;
  deviceId: string | null = null;
  errorMessage = '';
  successMessage = '';

  device: CreateDevice = {
    name: '',
    manufacturer: '',
    type: '',
    os: '',
    osVersion: '',
    processor: '',
    ram: 0,
    description: ''
  };

  constructor(
    private deviceService: DeviceService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.deviceId = this.route.snapshot.paramMap.get('id');
    this.isEditMode = !!this.deviceId;

    if (this.isEditMode && this.deviceId) {
      this.deviceService.getById(this.deviceId).subscribe({
        next: (device) => {
          this.device = {
            name: device.name,
            manufacturer: device.manufacturer,
            type: device.type,
            os: device.os,
            osVersion: device.osVersion,
            processor: device.processor,
            ram: device.ram,
            description: device.description
          };
        },
        error: () => {
          this.errorMessage = 'Failed to load device.';
        }
      });
    }
  }

  onDeviceChange(device: CreateDevice): void {
    this.device = device;
  }

  onSubmit(): void {
    if (!this.device.name || !this.device.manufacturer || !this.device.type ||
        !this.device.os || !this.device.osVersion || !this.device.processor || !this.device.ram) {
      this.errorMessage = 'All fields are required.';
      return;
    }

    if (this.isEditMode && this.deviceId) {
      this.deviceService.update(this.deviceId, this.device).subscribe({
        next: () => {
          this.router.navigate(['/home']);
        },
        error: () => {
          this.errorMessage = 'Failed to update device.';
        }
      });
    } else {
      this.deviceService.create(this.device).subscribe({
        next: () => {
          this.router.navigate(['/home']);
        },
        error: (err) => {
          this.errorMessage = err?.error || 'Failed to create device.';
        }
      });
    }
  }

  onCancel(): void {
    this.router.navigate(['/home']);
  }
}