import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { League, CreateLeague, UpdateLeague } from '../models/league.model';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class LeagueService {
  private readonly apiUrl = `${environment.apiUrl}/leagues`;

  constructor(private http: HttpClient) {}

  getAll(isActive?: boolean): Observable<League[]> {
    let params = new HttpParams();
    if (isActive !== undefined) params = params.set('isActive', isActive);
    return this.http.get<League[]>(this.apiUrl, { params });
  }

  getById(id: string): Observable<League> {
    return this.http.get<League>(`${this.apiUrl}/${id}`);
  }

  create(dto: CreateLeague): Observable<League> {
    return this.http.post<League>(this.apiUrl, dto);
  }

  update(id: string, dto: UpdateLeague): Observable<League> {
    return this.http.put<League>(`${this.apiUrl}/${id}`, dto);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
