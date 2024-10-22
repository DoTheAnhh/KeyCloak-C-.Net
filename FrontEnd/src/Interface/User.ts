export interface UserResponse {
  id: string;
  username: string;
}

export interface UserRequest {
  username?: string;
  email?: string;
  emailVerified?: boolean;
  enabled?: boolean;
  firstName?: string;
  lastName?: string;
  requiredActions?: string[];
  groups?: string[];
}
