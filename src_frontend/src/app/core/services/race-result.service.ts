import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { RaceResult, CreateRaceResult, UpdateRaceResult } from '../models/race-result.model';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class RaceResultService {
  private readonly apiUrl = `${environment.apiUrl}/races`;

  constructor(private http: HttpClient) {}

  getAll(raceId: string): Observable<RaceResult[]> {
    return this.http.get<RaceResult[]>(`${this.apiUrl}/${raceId}/results`);
  }

  getById(raceId: string, resultId: string): Observable<RaceResult> {
    return this.http.get<RaceResult>(`${this.apiUrl}/${raceId}/results/${resultId}`);
  }

  create(raceId: string, dto: CreateRaceResult): Observable<RaceResult> {
    return this.http.post<RaceResult>(`${this.apiUrl}/${raceId}/results`, dto);
  }

  update(raceId: string, resultId: string, dto: UpdateRaceResult): Observable<RaceResult> {
    return this.http.put<RaceResult>(`${this.apiUrl}/${raceId}/results/${resultId}`, dto);
  }

  delete(raceId: string, resultId: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${raceId}/results/${resultId}`);
  }
}
