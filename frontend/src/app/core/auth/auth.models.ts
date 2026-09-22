export type Role = 'Admin' | 'Buyer' | 'Seller';

export interface AuthUser {
  id: string;
  username: string;
  email: string;
  role: Role;
  storeName?: string;
}

/** POST /api/auth/login request body. `identifier` accepts an email or a username. */
export interface LoginRequest {
  identifier: string;
  password: string;
}

/** POST /api/auth/register request body */
export interface RegisterRequest {
  username: string;
  email: string;
  password: string;
  role: Extract<Role, 'Buyer' | 'Seller'>;
  storeName?: string;
}

/** Shared response shape for both /auth/login and /auth/register */
export interface AuthResponse {
  token: string;
  role: Role;
  userId?: string;
  username?: string;
  email?: string;
  storeName?: string;
}
