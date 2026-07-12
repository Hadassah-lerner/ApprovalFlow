# ADR-002 - Approval Service as Workflow Orchestrator

## Status:
Accepted

## Context:
The approval workflow includes asynchronous processing, AI analysis, policy validation, optional human approval, payment execution, and notification.
Without a central coordinator, asynchronous services may execute in an invalid order, which may cause inconsistent workflow processing (for example, payment before approval).

## Decision:
The *Approval Service* acts as the single orchestration point responsible for coordinating the workflow lifecycle.
It controls the workflow state, determines which step executes next, and coordinates the workflow lifecycle, determines the next processing step, and initiates payment for approved invoices.

## Alternatives Considered:
*Separate Workflow Service*
Rejected because it introduces unnecessary operational complexity without providing additional business value for the scope of this project.

*Distributed Choreography*
Rejected because no single component owns the workflow state, making debugging, pause/resume, retries, and workflow reasoning significantly more difficult.

## Consequences:
*Positive:*
* Single source of truth.
* Easier debugging.
* Easier monitoring.
* Supports durable pause/resume.
* Simplifies workflow management.
*Negative:*
* More responsibility concentrated in one service.
* Less distributed responsibility.
