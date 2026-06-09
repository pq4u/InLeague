import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Race, CreateRace, UpdateRace } from '../models/race.model';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class RaceService {
  private readonly apiUrl = `${environment.apiUrl}/leagues`;

  constructor(private http: HttpClient) {}

  getAll(leagueId: string): Observable<Race[]> {
    return this.http.get<Race[]>(`${this.apiUrl}/${leagueId}/races`);
  }

  getById(leagueId: string, raceId: string): Observable<Race> {
    return this.http.get<Race>(`${this.apiUrl}/${leagueId}/races/${raceId}`);
  }

  create(leagueId: string, dto: CreateRace): Observable<Race> {
    return this.http.post<Race>(`${this.apiUrl}/${leagueId}/races`, dto);
  }

  update(leagueId: string, raceId: string, dto: UpdateRace): Observable<Race> {
    return this.http.put<Race>(`${this.apiUrl}/${leagueId}/races/${raceId}`, dto);
  }

  delete(leagueId: string, raceId: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${leagueId}/races/${raceId}`);
  }
}
