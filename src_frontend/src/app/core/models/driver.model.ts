export interface Driver {
  id: string;
  firstName: string;
  lastName: string;
  fullName: string;
  racingNumber?: string;
  dateOfBirth?: string;
  createdAt: string;
}

export interface CreateDriver {
  firstName: string;
  lastName: string;
  racingNumber?: string;
  dateOfBirth?: string;
}

export interface UpdateDriver {
  firstName?: string;
  lastName?: string;
  racingNumber?: string;
  dateOfBirth?: string;
}
