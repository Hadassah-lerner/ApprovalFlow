import { createContext } from 'react'
import type {
  ApiError,
  ApprovalInvoice,
  InvoiceSubmissionResponse,
  SubmitInvoicePayload,
  WorkflowInvoice,
} from '../types/invoice'

export interface InvoiceLifecycleContextValue {
  approvals: ApprovalInvoice[]
  workflow: WorkflowInvoice[]
  loadingApprovals: boolean
  loadingWorkflow: boolean
  error: ApiError | null
  refreshApprovals: () => Promise<void>
  refreshWorkflow: () => Promise<void>
  submitNewInvoice: (payload: SubmitInvoicePayload) => Promise<InvoiceSubmissionResponse>
  approvePendingInvoice: (id: string) => Promise<void>
}

export const InvoiceLifecycleContext =
  createContext<InvoiceLifecycleContextValue | undefined>(undefined)
