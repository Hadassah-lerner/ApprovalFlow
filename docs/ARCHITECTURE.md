# ARCHITECTURE.md

# ApprovalFlow – High Level Architecture
**Version:** 1.0
**Project:** ApprovalFlow – AI Assisted Invoice & Expense Approval

---

# 1. Purpose
ApprovalFlow is an AI-assisted, microservice-based SaaS platform for invoice and expense approvals.

The system automatically approves low-risk requests while routing uncertain, high-value or policy-sensitive requests to a human approver.

The architecture prioritizes reliability, auditability and business rule enforcement over maximum AI autonomy.

The solution is designed to satisfy all Must-Have requirements while remaining realistic for implementation by a single developer within the project timeline.

---

# 2. Architecture Goals
The architecture was designed according to the following goals:

* Asynchronous request processing
* Loose coupling between business capabilities
* Clear service boundaries
* Human-in-the-loop support
* Safe AI integration
* Configurable business policies
* Reliable payment execution
* Full auditability
* Easy local deployment
* Technology simplicity over unnecessary complexity

---

# 3. Architectural Principles
The system follows the following design principles:

* Microservice Architecture
* Event-Driven Communication
* High Cohesion
* Loose Coupling
* One Database per Service
* Database Ownership
* Saga Pattern
* Idempotent Processing
* AI Recommendation Only
* Policy-Driven Decisions
* Structured Logging
* Correlation ID on every request

---

# 4. Autonomy Posture (Project Dilemma)
The project adopts a **Risk-Averse** AI posture.

The AI is never allowed to make the final business decision.

Automatic approval is granted only when **all** of the following conditions are satisfied:

* Invoice amount is below the configured approval ceiling.
* Expense category allows autonomous approval.
* Vendor passes validation.
* AI confidence is above the configured threshold.
* No duplicate submission is detected.
* No risk flags are returned.

Otherwise, the request is escalated to a human approver.

This guarantees compliance with requirement **M12**.

---

# 5. High-Level Architecture
The solution consists of four business microservices.

## API Gateway
Responsibilities

* Single external entry point
* Request routing
* Rate limiting
* Correlation ID generation
* Future authentication

---

## Submission Service
Responsibilities

* Receive invoice submissions
* Validate input
* Generate Tracking ID
* Perform idempotency validation
* Persist invoice
* Publish InvoiceSubmitted event

Owns

* Invoice data

---

## Approval Service (Workflow Orchestrator)
Responsibilities

* Consume InvoiceSubmitted events
* Retrieve approval policy
* Invoke AI provider
* Evaluate business rules
* Decide Auto Approval vs Human Approval
* Pause workflow
* Resume workflow
* Coordinate Payment Saga
* Publish workflow events

The Approval Service is the orchestration layer of the system.

It contains:

* AI Adapter
* Policy Evaluation
* Workflow Engine
* Human Approval Logic
* Saga Coordinator

Owns

* Approval workflow state

---

## Payment Service
Responsibilities

* Reserve payment
* Execute payment
* Retry failed payments
* Execute compensation
* Publish payment events

Owns

* Payment state

---

## Platform Service
Responsibilities

* Notification delivery
* Audit trail collection
* Business event history

Owns

* Notifications
* Audit records

---

# 6. Service Communication

## Synchronous Communication (Dapr Service Invocation)
Approval Service → AI Adapter

Approval Service → Policy Provider

---

## Asynchronous Communication (Dapr Pub/Sub)
Submission → Approval

Approval → Payment

Approval → Platform

Payment → Platform

---

# 7. Data Ownership
Submission Service

* Invoice
* Tracking ID
* Idempotency Key

Approval Service

* Approval State
* Human Review State
* AI Recommendation

Payment Service

* Payment
* Reservation
* Compensation Status

Platform Service

* Notifications
* Audit Trail

Every service owns its own data.

No service writes directly into another service's database.

---

# 8. Workflow
1. Client submits invoice.

2. Submission Service validates request.

3. Tracking ID is returned immediately.

4. InvoiceSubmitted event is published.

5. Approval Service consumes the event.

6. Policy is loaded.

7. AI evaluates the invoice.

8. Business rules validate AI output.

9. Decision:

* Auto Approve

or

* Human Review

10. Approved invoices trigger Payment Service.

11. Payment result generates notification.

12. Every important event is recorded in the audit history.

---

# 9. AI Decision Pipeline
Input

* Invoice
* Amount
* Vendor
* Category
* Description
* Company Policy

Output

```json
{
  "recommendation": "Approve",
  "confidence": 0.91,
  "reason": "...",
  "riskFlags": [],
  "policyClauses": [
    "Travel <= $150"
  ]
}
```

The Approval Service never trusts the AI blindly.

Every recommendation is validated against company policy before any business decision is made.

---

# 10. Idempotency Strategy
Duplicate submissions are prevented before entering the approval workflow.

Each request receives an Idempotency Key.

If the same request is received again:

* the original Tracking ID is returned;
* no second approval process starts;
* no duplicate payment can occur.

This satisfies F3 and M10.

---

# 11. Payment Saga
The payment process follows the Saga pattern.

Workflow

Invoice Approved

↓

Reserve Payment

↓

Execute Payment

↓

Success

If payment fails:

Retry

↓

Compensation

↓

Manual Finance Review

Compensation guarantees that no orphaned reservations remain in the system.

---

# 12. Dapr Components
The project uses Dapr as the distributed application runtime.

| Component          | Purpose                               |
| ------------------ | ------------------------------------- |
| Service Invocation | Synchronous service communication     |
| Pub/Sub            | RabbitMQ messaging                    |
| State Store        | Redis                                 |
| Configuration      | Approval policy                       |
| Secrets            | Connection strings & AI configuration |

---

# 13. Technology Stack
Backend

* .NET

Frontend

* React

Messaging

* RabbitMQ

Distributed Runtime

* Dapr

State Store

* Redis

Database

* PostgreSQL

API Gateway

* YARP

Containers

* Docker Compose

AI Provider

* Ollama

CI

* GitHub Actions

---

# 14. Cross-Cutting Concerns
The architecture implements the following cross-cutting capabilities:

* Correlation ID
* Structured Logging
* Health Checks
* Error Handling
* Retry Policies
* Idempotency
* Audit Trail
* Provider Swappability
* Configuration without Redeployment

---

# 15. Risks and Mitigations
| Risk                | Mitigation                          |
| ------------------- | ----------------------------------- |
| AI hallucination    | Policy always overrides AI          |
| Duplicate requests  | Idempotency                         |
| Payment failure     | Saga compensation                   |
| Service restart     | Persistent workflow state           |
| Policy changes      | External configuration              |
| AI provider failure | Fail-fast + explicit error handling |

---

# 16. Future Extensions
The architecture intentionally allows future expansion without major redesign.

Potential improvements include:

* JWT Authentication
* OpenTelemetry
* RAG over company policy
* Kubernetes deployment
* MCP Server
* Dashboard & BI
* Outbox Pattern
* Distributed Tracing

---

# 17. Architecture Summary
The architecture favors simplicity, maintainability and correctness over unnecessary microservice granularity.

Business rules remain deterministic.

AI improves efficiency but never compromises governance.

The Approval Service acts as the Workflow Orchestrator, ensuring that every approval, escalation and payment follows a deterministic, auditable and policy-compliant process.
