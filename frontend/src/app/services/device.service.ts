import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../environments/environment';
import { Device, CreateDevice } from '../models/device.model';

@Injectable({
  providedIn: 'root'
})
export class DeviceService {
  private apiUrl = `${environment.apiUrl}/devices`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<Device[]> {
    return this.http.get<Device[]>(this.apiUrl);
  }

  getById(id: string): Observable<Device> {
    return this.http.get<Device>(`${this.apiUrl}/${id}`);
  }

  create(device: CreateDevice): Observable<any> {
  return this.http.post(this.apiUrl, device, { responseType: 'text' });
}

  update(id: string, device: CreateDevice): Observable<any> {
  return this.http.put(`${this.apiUrl}/${id}`, device, { responseType: 'text' });
}

  delete(id: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }

  assign(id: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/${id}/assign`, {});
  }

  unassign(id: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/${id}/unassign`, {});
  }

  chat(message: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/chat`, { message });
  }

  search(query: string): Observable<Device[]> {
    return this.http.get<Device[]>(`${this.apiUrl}/search?q=${encodeURIComponent(query)}`);
  } 

}