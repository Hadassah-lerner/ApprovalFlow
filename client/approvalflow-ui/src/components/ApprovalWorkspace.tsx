import { useState } from 'react'
import { useInvoiceLifecycle } from '../hooks/useInvoiceLifecycle'
import type { ApprovalInvoice } from '../types/invoice'

function urgencyClass(urgency: string) {
  switch (urgency.toLowerCase()) {
    case 'high':
      return 'urgency urgency-high'
    case 'medium':
      return 'urgency urgency-medium'
    default:
      return 'urgency urgency-low'
  }
}

function formatCurrency(invoice: ApprovalInvoice) {
  return new Intl.NumberFormat(undefined, {
    style: 'currency',
    currency: invoice.currency,
  }).format(invoice.total)
}

export function ApprovalWorkspace() {
  const { approvals, approvePendingInvoice, loadingApprovals, refreshApprovals } =
    useInvoiceLifecycle()
  const [approvingId, setApprovingId] = useState<string | null>(null)
  const [actionError, setActionError] = useState<string | null>(null)

  const humanReviewInvoices = (approvals ?? []).filter((invoice) => {
    const status = invoice.approvalStatus?.toString().toLowerCase() ?? ''
    return status === 'humanreview'
  })

  async function handleApprove(id: string) {
    setApprovingId(id)
    setActionError(null)

    try {
      await approvePendingInvoice(id)
    } catch (error) {
      setActionError(error instanceof Error ? error.message : 'Approval failed.')
    } finally {
      setApprovingId(null)
    }
  }

  return (
    <section className="approval-section">
      <div className="section-heading">
        <p className="eyebrow">Approval Service</p>
        <h2>Approval Workspace</h2>
        <p>Review pending invoices with Ollama triage before releasing the payment saga.</p>
      </div>

      <div className="section-actions">
        <button className="secondary-button" type="button" onClick={() => void refreshApprovals()}>
          Refresh
        </button>
      </div>

      {actionError && <p className="error-banner">{actionError}</p>}

      {loadingApprovals ? (
        <div className="empty-state">Loading human-review cases...</div>
      ) : humanReviewInvoices.length === 0 ? (
        <div className="empty-state">No invoices are currently waiting for human review.</div>
      ) : (
        <div className="approval-grid">
          {humanReviewInvoices.map((invoice) => (
            <article className="approval-card" key={invoice.id}>
              <div className="card-topline">
                <span className="tracking-id">{invoice.trackingId || invoice.invoiceNumber}</span>
                <span className={urgencyClass(invoice.aiUrgencyLevel || 'Low')}>
                  {invoice.aiUrgencyLevel || 'Low'} urgency
                </span>
              </div>

              <div>
                <h3>{invoice.vendor}</h3>
                <p className="invoice-meta">
                  {invoice.department} / {invoice.invoiceNumber}
                </p>
              </div>

              <div className="invoice-amount">{formatCurrency(invoice)}</div>

              <div className="ai-insights">
                <div>
                  <span>Suggested Category</span>
                  <strong>{invoice.aiSuggestedCategory || 'Unclassified'}</strong>
                </div>
                <blockquote>
                  {invoice.aiReasoning ||
                    'AI triage has not returned reasoning for this invoice yet.'}
                </blockquote>
              </div>

              <button
                className="primary-button approve-button"
                disabled={approvingId === invoice.id}
                type="button"
                onClick={() => void handleApprove(invoice.id)}
              >
                {approvingId === invoice.id ? 'Approving...' : 'Approve Invoice'}
              </button>
            </article>
          ))}
        </div>
      )}
    </section>
  )
}
