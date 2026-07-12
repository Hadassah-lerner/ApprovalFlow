import { useContext } from 'react'
import { InvoiceLifecycleContext } from '../context/lifecycleContextValue'
import type { InvoiceSubmissionResponse } from '../types/invoice'

export function useInvoiceLifecycle() {
  const context = useContext(InvoiceLifecycleContext)

  if (!context) {
    return {
      approvals: [],
      workflow: [],
      loadingApprovals: false,
      loadingWorkflow: false,
      error: null,
      refreshApprovals: async () => undefined,
      refreshWorkflow: async () => undefined,
      submitNewInvoice: async (): Promise<InvoiceSubmissionResponse> => ({}),
      approvePendingInvoice: async () => undefined,
    }
  }

  return context
}