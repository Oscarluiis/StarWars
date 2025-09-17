import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { config } from './app/config';

@Injectable({
    providedIn: 'root'
})
export class AuthService {

    constructor(private http: HttpClient) { }

    register(request: any): Observable<any> {
        return this.http.post(`${config.apiUrl}/Auth/register`, request);
    }

    login(request: any): Observable<any> {
        return this.http.post(`${config.apiUrl}/Auth/login`, request);
    }

    logout(): void {
        localStorage.removeItem('authToken');
        localStorage.removeItem('currentUser');
    }

    isAuthenticated(): boolean {
        return !!localStorage.getItem('authToken');
    }
}