export interface CreateCustomerRequest {
  firstName: string;
  lastName: string;
  email: string;
  phone: string | null;
  acceptsMarketing: boolean;
  note: string | null;
}

export interface CreateCustomerResponse {
  customerId: string;
  shopifySyncSucceeded: boolean;
  shopifyCustomerId: string | null;
  shopifySyncError: string | null;
}

export interface ProblemDetails {
  title?: string;
  detail?: string;
  status?: number;
  errors?: Record<string, string[]>;
}
