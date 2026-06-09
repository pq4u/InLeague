export interface Kart {
  id: string;
  number: string;
  model?: string;
  category?: string;
  isActive: boolean;
}

export interface CreateKart {
  number: string;
  model?: string;
  category?: string;
}

export interface UpdateKart {
  number?: string;
  model?: string;
  category?: string;
  isActive?: boolean;
}
