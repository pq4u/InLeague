import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Kart, CreateKart, UpdateKart } from '../models/kart.model';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class KartService {
  private readonly apiUrl = `${environment.apiUrl}/karts`;

  constructor(private http: HttpClient) {}

  getAll(isActive?: boolean): Observable<Kart[]> {
    let params = new HttpParams();
    if (isActive !== undefined) {
      params = params.set('isActive', isActive);
    }
    return this.http.get<Kart[]>(this.apiUrl, { params });
  }

  getById(id: string): Observable<Kart> {
    return this.http.get<Kart>(`${this.apiUrl}/${id}`);
  }

  create(dto: CreateKart): Observable<Kart> {
    return this.http.post<Kart>(this.apiUrl, dto);
  }

  update(id: string, dto: UpdateKart): Observable<Kart> {
    return this.http.put<Kart>(`${this.apiUrl}/${id}`, dto);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
