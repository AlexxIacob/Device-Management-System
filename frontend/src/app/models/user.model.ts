export interface User {
  id: string;
  name: string;
  role: string;
  location: string;
  email: string;
}

export interface UpdateProfile {
  name: string;
  role: string;
  location: string;
}