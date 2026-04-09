import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Device } from '../../models/device.model';

@Component({
  selector: 'app-device-table',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './device-table.html',
  styleUrl: './device-table.css'
})
export class DeviceTableComponent {
  @Input() devices: Device[] = [];
  @Input() currentUserId: string | null = null;
  @Input() showEdit = true;
  @Input() showDelete = true;
  @Input() showAssign = true;
  @Output() viewDevice = new EventEmitter<string>();
  @Output() editDevice = new EventEmitter<string>();
  @Output() deleteDevice = new EventEmitter<string>();
  @Output() assignDevice = new EventEmitter<string>();
  @Output() unassignDevice = new EventEmitter<string>();

  isAssignedToCurrentUser(device: Device): boolean {
    return device.assignedUserId === this.currentUserId;
  }

  isFree(device: Device): boolean {
    return !device.assignedUserId;
  }
}