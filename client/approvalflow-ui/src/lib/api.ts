import type {
  ApprovalInvoice,
  InvoiceSubmissionResponse,
  SubmitInvoicePayload,
  SubmitInvoiceRequest,
  WorkflowInvoice,
} from '../types/invoice'

const jsonHeaders = {
  'Content-Type': 'application/json',
}

function unwrapCloudEvent<T>(payload: unknown): T {
  if (
    payload &&
    typeof payload === 'object' &&
    'data' in payload &&
    ('specversion' in payload || 'type' in payload || 'source' in payload)
  ) {
    return unwrapCloudEvent<T>((payload as { data: unknown }).data)
  }

  return payload as T
}

function unwrapCollection<T>(payload: unknown): T[] {
  const unwrapped = unwrapCloudEvent<unknown>(payload)

  if (Array.isArray(unwrapped)) {
    return unwrapped as T[]
  }

  if (unwrapped && typeof unwrapped === 'object') {
    const candidate = unwrapped as Record<string, unknown>
    const collection = candidate.items ?? candidate.results ?? candidate.data ?? candidate.invoices

    if (Array.isArray(collection)) {
      return collection as T[]
    }
  }

  return []
}

async function request<T>(path: string, init?: RequestInit): Promise<T> {
  const response = await fetch(path, init)
  const contentType = response.headers.get('content-type') ?? ''
  const payload = contentType.includes('application/json') ? await response.json() : await response.text()

  if (!response.ok) {
    const message =
      payload && typeof payload === 'object' && 'message' in payload
        ? String((payload as { message: unknown }).message)
        : String(payload || `Request failed with status ${response.status}`)

    throw Object.assign(new Error(message), { status: response.status })
  }

  return unwrapCloudEvent<T>(payload)
}

function toSubmissionRequest(payload: SubmitInvoicePayload): SubmitInvoiceRequest {
  // המרת המטבע מגרסת הטקסט למספר Enum עבור הבקאנד (0 = USD, 1 = ILS)
  const currencyEnum = payload.currency === 'ILS' ? 1 : 0;

  return {
    ...payload,
    currency: currencyEnum as any, // עוקף זמנית את הטיפוס החוצה לבקאנד כ-Enum מספרי
    submitter: 'Frontend Console',
    vendorKnown: true,
    category: 0 as any, // שליחת 0 (למשל קטגוריית General/Operations הראשונה ב-Enum של ה-C#)
    taxAmount: 0,
    receiptPresent: true,
    invoiceDate: new Date().toISOString(),
    notes: '',
  }
}

export async function submitInvoice(
  payload: SubmitInvoicePayload,
): Promise<InvoiceSubmissionResponse> {
  return request<InvoiceSubmissionResponse>('/submission/api/invoices', {
    method: 'POST',
    headers: jsonHeaders,
    body: JSON.stringify(toSubmissionRequest(payload)),
  })
}

export async function getPendingApprovals(): Promise<ApprovalInvoice[]> {
  const payload = await request<unknown>('/approval/api/invoices?status=HumanReview')
  return unwrapCollection<ApprovalInvoice>(payload)
}

export async function approveInvoice(id: string): Promise<void> {
  await request<void>(`/approval/api/invoices/${id}/approve`, {
    method: 'POST',
    headers: jsonHeaders,
  })
}

export async function getWorkflowInvoices(): Promise<WorkflowInvoice[]> {
  const payload = await request<unknown>('/submission/api/invoices')
  return unwrapCollection<WorkflowInvoice>(payload)
}