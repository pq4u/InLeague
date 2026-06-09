import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Driver, CreateDriver, UpdateDriver } from '../models/driver.model';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class DriverService {
  private readonly apiUrl = `${environment.apiUrl}/drivers`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<Driver[]> {
    return this.http.get<Driver[]>(this.apiUrl);
  }

  getById(id: string): Observable<Driver> {
    return this.http.get<Driver>(`${this.apiUrl}/${id}`);
  }

  create(dto: CreateDriver): Observable<Driver> {
    return this.http.post<Driver>(this.apiUrl, dto);
  }

  update(id: string, dto: UpdateDriver): Observable<Driver> {
    return this.http.put<Driver>(`${this.apiUrl}/${id}`, dto);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
