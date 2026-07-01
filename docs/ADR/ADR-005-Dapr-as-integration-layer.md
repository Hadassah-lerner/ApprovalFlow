# ADR-005 - Dapr as the Integration Layer

## Status:
Accepted

## Context:
The system depends on service-to-service communication, messaging, state management, and configuration.
Direct integration with infrastructure technologies such as RabbitMQ and Redis would tightly couple services to specific implementations, making infrastructure changes more expensive.

## Decision:
The system uses *Dapr* as the integration layer.
The following Dapr building blocks are used:
* Service Invocation
* Pub/Sub
* State Store
* Configuration

Application services communicate through Dapr APIs instead of vendor-specific SDKs.

## Alternatives Considered:
*Direct Infrastructure Integration*
Rejected because infrastructure changes would require application code modifications across multiple services, increasing maintenance effort and coupling.

## Consequences:
*Positive:*
* Loose coupling between services and !infrastructure.
* Infrastructure technologies can be replaced with minimal code changes.
* Improved maintainability.
* Cleaner service implementation.
*Negative:*
* Additional abstraction layer.
* Learning curve for developers unfamiliar with Dapr.

