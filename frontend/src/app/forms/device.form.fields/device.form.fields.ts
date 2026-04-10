import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CreateDevice } from '../../models/device.model';

@Component({
  selector: 'app-device-form-fields',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './device.form.fields.html',
  styleUrl: './device.form.fields.css'
})
export class DeviceFormFieldsComponent {
  @Input() device: CreateDevice = {
    name: '',
    manufacturer: '',
    type: '',
    os: '',
    osVersion: '',
    processor: '',
    ram: 0,
    description: ''
  };

  @Output() deviceChange = new EventEmitter<CreateDevice>();

  onFieldChange(): void {
    this.deviceChange.emit(this.device);
  }
}