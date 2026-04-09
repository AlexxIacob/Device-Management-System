import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { environment } from '../environments/environment';
import { UpdateProfile } from '../models/user.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = `${environment.apiUrl}/auth`;

  constructor(private http: HttpClient) {}

  register(email: string, password: string): Observable<any> {
  return this.http.post(`${this.apiUrl}/register`, { email, password }, { responseType: 'text' });
}

  login(email: string, password: string): Observable<any> {
  return this.http.post<{ token: string }>(`${this.apiUrl}/login`, { email, password }).pipe(
    tap(response => {
      localStorage.setItem('token', response.token);
    })
  );
}

  logout(): void {
    localStorage.removeItem('token');
  }

  updateProfile(data: UpdateProfile): Observable<any> {
    return this.http.put(`${this.apiUrl}/profile`, data);
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }

  isLoggedIn(): boolean {
    return this.getToken() !== null;
  }

  getUserId(): string | null {
    const token = this.getToken();
    if (!token) return null;

    const payload = JSON.parse(atob(token.split('.')[1]));
    return payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'];
  }
}