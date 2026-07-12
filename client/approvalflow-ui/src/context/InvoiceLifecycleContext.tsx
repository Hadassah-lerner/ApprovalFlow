import { useCallback, useEffect, useMemo, useState, type ReactNode } from 'react'
import {
  approveInvoice,
  getPendingApprovals,
  getWorkflowInvoices,
  submitInvoice,
} from '../lib/api'
import { InvoiceLifecycleContext } from './lifecycleContextValue'
import type {
  ApiError,
  ApprovalInvoice,
  SubmitInvoicePayload,
  WorkflowInvoice,
} from '../types/invoice'

function toApiError(error: unknown): ApiError {
  if (error instanceof Error) {
    return {
      message: error.message,
      status:
        'status' in error && typeof error.status === 'number'
          ? error.status
          : undefined,
    }
  }

  return { message: 'Unexpected API error' }
}

export function InvoiceLifecycleProvider({ children }: { children: ReactNode }) {
  const [approvals, setApprovals] = useState<ApprovalInvoice[]>([])
  const [workflow, setWorkflow] = useState<WorkflowInvoice[]>([])
  const [loadingApprovals, setLoadingApprovals] = useState(false)
  const [loadingWorkflow, setLoadingWorkflow] = useState(false)
  const [error, setError] = useState<ApiError | null>(null)

  const refreshApprovals = useCallback(async () => {
    setLoadingApprovals(true)
    setError(null)

    try {
      setApprovals(await getPendingApprovals())
    } catch (requestError) {
      setError(toApiError(requestError))
      setApprovals([])
    } finally {
      setLoadingApprovals(false)
    }
  }, [])

  const refreshWorkflow = useCallback(async () => {
    setLoadingWorkflow(true)
    setError(null)

    try {
      setWorkflow(await getWorkflowInvoices())
    } catch (requestError) {
      setError(toApiError(requestError))
      setWorkflow([])
    } finally {
      setLoadingWorkflow(false)
    }
  }, [])

  const submitNewInvoice = useCallback(
    async (payload: SubmitInvoicePayload) => {
      const response = await submitInvoice(payload)
      await Promise.all([refreshApprovals(), refreshWorkflow()])
      return response
    },
    [refreshApprovals, refreshWorkflow],
  )

  const approvePendingInvoice = useCallback(
    async (id: string) => {
      await approveInvoice(id)
      setApprovals((current) => current.filter((invoice) => invoice.id !== id))
      await Promise.all([refreshApprovals(), refreshWorkflow()])
    },
    [refreshApprovals, refreshWorkflow],
  )

  useEffect(() => {
    const timeoutId = window.setTimeout(() => {
      void Promise.all([refreshApprovals(), refreshWorkflow()])
    }, 0)

    return () => window.clearTimeout(timeoutId)
  }, [refreshApprovals, refreshWorkflow])

  const value = useMemo(
    () => ({
      approvals,
      workflow,
      loadingApprovals,
      loadingWorkflow,
      error,
      refreshApprovals,
      refreshWorkflow,
      submitNewInvoice,
      approvePendingInvoice,
    }),
    [
      approvals,
      workflow,
      loadingApprovals,
      loadingWorkflow,
      error,
      refreshApprovals,
      refreshWorkflow,
      submitNewInvoice,
      approvePendingInvoice,
    ],
  )

  return (
    <InvoiceLifecycleContext.Provider value={value}>
      {children}
    </InvoiceLifecycleContext.Provider>
  )
}
