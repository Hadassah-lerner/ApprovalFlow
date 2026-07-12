export type Currency = 'USD' | 'ILS'

export type WorkflowStatus = 'Submitted' | 'Processing' | 'Approved' | 'Paid' | 'PaymentFailed' | 'HumanReview';
export interface LineItemInput {
  description: string
  quantity: number
  unitPrice: number
}

export interface SubmitInvoicePayload {
  vendor: string
  department: string
  invoiceNumber: string
  total: number
  currency: Currency
  lineItems: LineItemInput[]
}

export interface SubmitInvoiceRequest extends SubmitInvoicePayload {
  submitter: string
  vendorKnown: boolean
  category: string
  taxAmount: number
  receiptPresent: boolean
  invoiceDate: string
  notes: string
}

export interface InvoiceSubmissionResponse {
  invoiceId?: string
  id?: string
  trackingId?: string
  status?: string
}

export interface ApprovalInvoice {
  id: string
  trackingId: string
  vendor: string
  department: string
  invoiceNumber: string
  total: number
  currency: Currency
  approvalStatus: string
  aiUrgencyLevel: string
  aiSuggestedCategory: string
  aiReasoning: string
  lineItems: LineItemInput[]
}

export interface WorkflowInvoice {
  id: string
  trackingId: string
  vendor: string
  invoiceNumber: string
  department: string
  total: number
  currency: Currency
  status: WorkflowStatus
  failureReason?: string
  updatedAt?: string
}

export interface ApiError {
  message: string
  status?: number
}
