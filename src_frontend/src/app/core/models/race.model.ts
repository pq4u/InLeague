import { RaceResult } from './race-result.model';

export interface Race {
  id: string;
  leagueId: string;
  name: string;
  location?: string;
  raceDate: string;
  numberOfLaps: number;
  notes?: string;
  resultCount: number;
  createdAt: string;
  results?: RaceResult[];
}

export interface CreateRace {
  name: string;
  location?: string;
  raceDate: string;
  numberOfLaps: number;
  notes?: string;
}

export interface UpdateRace {
  name?: string;
  location?: string;
  raceDate?: string;
  numberOfLaps?: number;
  notes?: string;
}
