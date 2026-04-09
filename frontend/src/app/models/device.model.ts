export interface Device {
  id: string;
  name: string;
  manufacturer: string;
  type: string;
  os: string;
  osVersion: string;
  processor: string;
  ram: number;
  description: string;
  assignedUserId?: string | null;
  assignedUserName?: string | null;
}

export interface CreateDevice {
  name: string;
  manufacturer: string;
  type: string;
  os: string;
  osVersion: string;
  processor: string;
  ram: number;
  description: string;
}