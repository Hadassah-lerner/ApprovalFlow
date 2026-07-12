# ADR-003 - Saga Pattern for Payment Consistency

## Status:
Accepted

## Context:
The payment process includes multiple services. Failures occurring after partial completed steps may leave the system in an inconsistent state, such as reserved funds without completed payment or duplicated payment attempts.

## Decision:
The system uses an *Orchestration-Based Saga.*
The system follows a simplified orchestration-based saga where the Approval Service coordinates the payment workflow through asynchronous events. Compensation logic is considered a future extension.

## Alternatives Considered:
*Simple Retry*
Rejected because retries cannot determine which operations across multiple services have already completed successfully and may produce duplicate effects.

*Distributed Transactions (Two-Phase Commit)*
Rejected because they introduce tighter coupling between services and reduce scalability in a microservice architecture.

## Consequences:
*Positive:*
* Distributed consistency.
* Automatic compensation.
* Reliable failure recovery.
* Prevents partial or duplicated payments.
*Negative:*
* Additional workflow complexity.
* More workflow logic to maintain.
