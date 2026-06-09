import { Race } from './race.model';

export interface League {
  id: string;
  name: string;
  description?: string;
  startDate: string;
  endDate?: string;
  isActive: boolean;
  raceCount: number;
  createdAt: string;
  races?: Race[];
}

export interface CreateLeague {
  name: string;
  description?: string;
  startDate: string;
  endDate?: string;
}

export interface UpdateLeague {
  name?: string;
  description?: string;
  startDate?: string;
  endDate?: string;
  isActive?: boolean;
}
