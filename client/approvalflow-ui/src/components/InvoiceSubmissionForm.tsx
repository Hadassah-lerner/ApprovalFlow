import { useMemo, useState, type FormEvent } from 'react'
import { useInvoiceLifecycle } from '../hooks/useInvoiceLifecycle'
import type { Currency, LineItemInput, SubmitInvoicePayload } from '../types/invoice'

const emptyLineItem: LineItemInput = {
  description: '',
  quantity: 1,
  unitPrice: 0,
}

const currencyOptions: Currency[] = ['USD', 'ILS']

export function InvoiceSubmissionForm() {
  const { submitNewInvoice } = useInvoiceLifecycle()
  const [vendor, setVendor] = useState('')
  const [department, setDepartment] = useState('')
  const [invoiceNumber, setInvoiceNumber] = useState('')
  const [currency, setCurrency] = useState<Currency>('USD')
  const [lineItems, setLineItems] = useState<LineItemInput[]>([{ ...emptyLineItem }])
  const [isSubmitting, setIsSubmitting] = useState(false)
  const [message, setMessage] = useState<string | null>(null)

  const calculatedTotal = useMemo(
    () =>
      lineItems.reduce(
        (sum, item) => sum + Number(item.quantity || 0) * Number(item.unitPrice || 0),
        0,
      ),
    [lineItems],
  )

  function updateLineItem(index: number, patch: Partial<LineItemInput>) {
    setLineItems((current) =>
      current.map((item, itemIndex) => (itemIndex === index ? { ...item, ...patch } : item)),
    )
  }

  function addLineItem() {
    setLineItems((current) => [...current, { ...emptyLineItem }])
  }

  function removeLineItem(index: number) {
    setLineItems((current) => current.filter((_, itemIndex) => itemIndex !== index))
  }

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setIsSubmitting(true)
    setMessage(null)

    const payload: SubmitInvoicePayload = {
      vendor,
      department,
      invoiceNumber,
      total: Number(calculatedTotal.toFixed(2)),
      currency,
      lineItems: lineItems.map((item) => ({
        description: item.description,
        quantity: Number(item.quantity),
        unitPrice: Number(item.unitPrice),
      })),
    }

    try {
      const response = await submitNewInvoice(payload)
      setMessage(`Invoice ${response.trackingId ?? response.invoiceId ?? invoiceNumber} submitted.`)
      setVendor('')
      setDepartment('')
      setInvoiceNumber('')
      setCurrency('USD')
      setLineItems([{ ...emptyLineItem }])
    } catch (error) {
      setMessage(error instanceof Error ? error.message : 'Invoice submission failed.')
    } finally {
      setIsSubmitting(false)
    }
  }

  return (
    <section className="panel invoice-form-panel">
      <div className="section-heading">
        <p className="eyebrow">Submission Service</p>
        <h2>Invoice Submission Dashboard</h2>
        <p>Capture incoming vendor invoices and publish them into the lifecycle.</p>
      </div>

      <form className="invoice-form" onSubmit={handleSubmit}>
        <div className="form-grid">
          <label className="field">
            <span>Vendor</span>
            <input
              required
              value={vendor}
              onChange={(event) => setVendor(event.target.value)}
              placeholder="Acme Cloud Ltd."
            />
          </label>

          <label className="field">
            <span>Department</span>
            <input
              required
              value={department}
              onChange={(event) => setDepartment(event.target.value)}
              placeholder="Finance Operations"
            />
          </label>

          <label className="field">
            <span>Invoice Number</span>
            <input
              required
              value={invoiceNumber}
              onChange={(event) => setInvoiceNumber(event.target.value)}
              placeholder="INV-2026-1042"
            />
          </label>

          <label className="field">
            <span>Currency</span>
            <select value={currency} onChange={(event) => setCurrency(event.target.value as Currency)}>
              {currencyOptions.map((option) => (
                <option key={option} value={option}>
                  {option}
                </option>
              ))}
            </select>
          </label>
        </div>

        <div className="line-items-header">
          <div>
            <p className="eyebrow">Line Items</p>
            <h3>Invoice Detail</h3>
          </div>
          <button className="secondary-button" type="button" onClick={addLineItem}>
            Add Row
          </button>
        </div>

        <div className="line-items">
          {lineItems.map((item, index) => (
            <div className="line-item-row" key={`${index}-${item.description}`}>
              <label className="field line-description">
                <span>Description</span>
                <input
                  required
                  value={item.description}
                  onChange={(event) => updateLineItem(index, { description: event.target.value })}
                  placeholder="Subscription, equipment, consulting..."
                />
              </label>

              <label className="field">
                <span>Qty</span>
                <input
                  min="1"
                  required
                  type="number"
                  value={item.quantity}
                  onChange={(event) => updateLineItem(index, { quantity: Number(event.target.value) })}
                />
              </label>

              <label className="field">
                <span>Unit Price</span>
                <input
                  min="0"
                  required
                  step="0.01"
                  type="number"
                  value={item.unitPrice}
                  onChange={(event) => updateLineItem(index, { unitPrice: Number(event.target.value) })}
                />
              </label>

              <button
                aria-label="Remove line item"
                className="icon-button"
                disabled={lineItems.length === 1}
                type="button"
                onClick={() => removeLineItem(index)}
              >
                x
              </button>
            </div>
          ))}
        </div>

        <div className="form-footer">
          <div>
            <span className="total-label">Total</span>
            <strong>
              {currency} {calculatedTotal.toLocaleString(undefined, { minimumFractionDigits: 2 })}
            </strong>
          </div>
          <button className="primary-button" disabled={isSubmitting} type="submit">
            {isSubmitting ? 'Submitting...' : 'Submit Invoice'}
          </button>
        </div>

{message && (
  <div 
    className={message.toLowerCase().includes('failed') || message.toLowerCase().includes('error') ? 'error-banner' : 'success-banner'}
  >
    {message}
  </div>
)}
      </form>
    </section>
  )
}
