export type ResultStatus = 'Finished' | 'DNF' | 'DNS' | 'Disqualified';

export interface RaceResult {
  id: string;
  raceId: string;
  driverId: string;
  driverName: string;
  kartId: string;
  kartNumber: string;
  lapTimeMs: number;
  totalTimeMs: number;
  startingPosition: number;
  finishingPosition?: number;
  lapsCompleted: number;
  status: ResultStatus;
  notes?: string;
}

export interface CreateRaceResult {
  driverId: string;
  kartId: string;
  lapTimeMs: number;
  totalTimeMs: number;
  startingPosition: number;
  finishingPosition?: number;
  lapsCompleted: number;
  status: ResultStatus;
  notes?: string;
}

export interface UpdateRaceResult {
  driverId?: string;
  kartId?: string;
  lapTimeMs?: number;
  totalTimeMs?: number;
  startingPosition?: number;
  finishingPosition?: number;
  lapsCompleted?: number;
  status?: ResultStatus;
  notes?: string;
}
