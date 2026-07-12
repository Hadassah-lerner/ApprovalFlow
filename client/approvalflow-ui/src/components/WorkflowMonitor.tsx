import { useInvoiceLifecycle } from '../hooks/useInvoiceLifecycle'
import type { WorkflowInvoice } from '../types/invoice'

const statusLabels: Record<string, string> = {
  submitted: 'Processing AI...',
  processing: 'Processing AI...',
  pending: 'Pending Review',
  humanreview: 'Pending Human Review',
  approved: 'Approved',
  paid: 'Paid Settlement',
  paymentfailed: 'Payment Failed',
  rejected: 'Rejected',
}

function statusClass(status: string) {
  const s = status?.toLowerCase() || 'submitted'
  return `status-pill status-${s}`
}

function normalizeStatus(invoice: Record<string, unknown>) {
  const candidates = [invoice.status, invoice.approvalStatus, invoice.sagaStatus]

  for (const candidate of candidates) {
    if (typeof candidate === 'string' && candidate.trim()) {
      return candidate.trim()
    }

    if (typeof candidate === 'number') {
      return String(candidate)
    }
  }

  return 'submitted'
}

function normalizeCurrency(value: unknown) {
  if (typeof value === 'string') {
    const lower = value.toLowerCase()
    if (lower === 'ils') return 'ILS'
    if (lower === 'usd') return 'USD'
  }

  if (typeof value === 'number') {
    return value === 1 ? 'ILS' : 'USD'
  }

  return 'USD'
}

function normalizeInvoice(invoice: Record<string, unknown>): WorkflowInvoice {
  return {
    id: String(invoice.id ?? ''),
    trackingId: String(invoice.trackingId ?? invoice.id ?? 'TRK-PENDING'),
    vendor: String(invoice.vendor ?? 'Unknown Vendor'),
    invoiceNumber: String(invoice.invoiceNumber ?? 'N/A'),
    department: String(invoice.department ?? ''),
    total: Number(invoice.total ?? 0),
    currency: normalizeCurrency(invoice.currency) as WorkflowInvoice['currency'],
    status: normalizeStatus(invoice) as WorkflowInvoice['status'],
    updatedAt: typeof invoice.updatedAt === 'string' ? invoice.updatedAt : undefined,
  }
}

function formatAmount(invoice: WorkflowInvoice) {
  return new Intl.NumberFormat(undefined, {
    style: 'currency',
    currency: invoice.currency || 'USD',
  }).format(invoice.total)
}

export function WorkflowMonitor() {
  const { workflow = [], loadingWorkflow, refreshWorkflow } = useInvoiceLifecycle()

  const safeWorkflow = Array.isArray(workflow) ? workflow : []
  const normalizedWorkflow = safeWorkflow.map((invoice) => normalizeInvoice(invoice as unknown as Record<string, unknown>))

  return (
    <section className="panel workflow-panel">
      <div className="section-heading">
        <p className="eyebrow">Platform Service</p>
        <h2>Platform Workflow Monitor</h2>
        <p>Track the cross-service saga state directly from the source of truth.</p>
      </div>

      <div className="section-actions">
        <button className="secondary-button" type="button" onClick={() => void refreshWorkflow()}>
          Refresh Status
        </button>
      </div>

      <div className="workflow-table-wrap">
        <table className="workflow-table">
          <thead>
            <tr>
              <th>Tracking ID</th>
              <th>Vendor</th>
              <th>Invoice #</th>
              <th>Amount</th>
              <th>Status</th>
              <th>Updated At</th>
            </tr>
          </thead>

          <tbody>
            {loadingWorkflow ? (
              <tr>
                <td colSpan={6}>Loading live saga states...</td>
              </tr>
            ) : normalizedWorkflow.length === 0 ? (
              <tr>
                <td colSpan={6}>No active saga workflows found.</td>
              </tr>
            ) : (
              normalizedWorkflow.map((invoice, index) => {
                const rawStatus = invoice.status?.toString().toLowerCase() || 'submitted'

                return (
                  <tr key={`${invoice.id || invoice.trackingId || index}`}>
                    <td>{invoice.trackingId || 'TRK-PENDING'}</td>
                    <td>{invoice.vendor}</td>
                    <td>{invoice.invoiceNumber || 'N/A'}</td>
                    <td>{formatAmount(invoice)}</td>
                    <td>
                      <span className={statusClass(rawStatus)}>
                        {statusLabels[rawStatus] || invoice.status}
                      </span>
                    </td>
                    <td>
                      {invoice.updatedAt
                        ? new Intl.DateTimeFormat(undefined, {
                            dateStyle: 'medium',
                            timeStyle: 'short',
                          }).format(new Date(invoice.updatedAt))
                        : 'Just Now'}
                    </td>
                  </tr>
                )
              })
            )}
          </tbody>
        </table>
      </div>
    </section>
  )
}