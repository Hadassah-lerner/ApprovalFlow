# ApprovalFlow – AI-Assisted Invoice Approval System

ApprovalFlow is a **proof-of-concept microservice application** demonstrating an AI-assisted invoice approval workflow built with **.NET**, **React**, **Dapr**, and **RabbitMQ**.

The project focuses on **event-driven communication**, **service separation**, and **AI-assisted decision making**, rather than production-ready financial processing.

---

# Architecture

The solution consists of **three independent microservices** and a single API Gateway.

## Submission Service

**Responsibilities**

- Receive invoice submissions
- Validate incoming requests
- Generate Tracking IDs
- Persist invoices
- Publish `InvoiceSubmitted` events

---

## Approval Service

**Responsibilities**

- Consume `InvoiceSubmitted` events
- Load approval policy
- Execute Ollama AI classification
- Evaluate business rules
- Route invoices to:
  - Automatic Approval
  - Human Review
- Publish `PaymentRequested` events

---

## Payment Service

**Responsibilities**

- Consume payment requests
- Simulate payment processing
- Persist payment information
- Publish payment completion events

---

## API Gateway

YARP Reverse Proxy acting as the single entry point between the frontend and backend services.

---

# Workflow

```text
Invoice Submitted
        │
        ▼
Submission Service
        │
InvoiceSubmitted Event
        │
        ▼
Approval Service
        │
AI Classification
        │
Policy Evaluation
        │
 ┌───────────────┐
 │               │
 ▼               ▼
Auto Approve   Human Review
 │
 ▼
Payment Service
 │
 ▼
Payment Completed
```

---

# Technologies

| Layer | Technology |
|--------|------------|
| Backend | .NET Web API, Entity Framework Core |
| Frontend | React, TypeScript, Vite |
| Messaging | Dapr Pub/Sub, RabbitMQ |
| Database | PostgreSQL |
| AI | Ollama (Local) |
| Gateway | YARP |
| Infrastructure | Docker Compose |

---

# Current Features

- Invoice submission
- Invoice validation
- Event-driven communication
- AI-assisted invoice classification
- Policy-based approval routing
- Payment simulation
- Invoice status tracking
- Swagger APIs
- Dockerized infrastructure

---

# Known Limitations

This project was developed as an **academic proof-of-concept**.

The current implementation demonstrates the overall architecture and event-driven workflow.

The following production features are intentionally simplified:

- Workflow state synchronization between services
- Human approval persistence
- Distributed Saga recovery
- Retry and compensation logic
- Audit history

---

# Running the Project

```bash
docker compose up --build
```

---

# Service Endpoints

| Service | URL |
|---------|-----|
| Frontend | http://localhost:5173 |
| API Gateway | http://localhost:5251 |
| Submission Swagger | http://localhost:5251/submission/submission/swagger/index.html |
| Approval Swagger | http://localhost:5251/approval/approval/swagger/index.html |

---

# Architecture Highlights

- Event-driven microservices
- Database-per-service architecture
- AI-assisted approval decisions
- Policy-based routing
- Dapr Pub/Sub messaging
- Clean Architecture
- Independent service boundaries

> **Architecture Note**
>
> The diagrams below illustrate the intended target architecture and end-to-end workflow of the project. The current implementation demonstrates the core concepts and most of the processing pipeline, while some advanced capabilities remain planned for future iterations.
