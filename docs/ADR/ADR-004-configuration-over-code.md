# ADR-004 - Configuration over Code

## Status:
Accepted

## Context:
Business policies and approval thresholds change more frequently than application code. Requiring code modifications and redeployment for every policy update would slow down business operations.

## Decision:
Approval thresholds and policy rules are loaded at runtime from external configuration.
Business users (such as Finance) can update approval rules without requiring application code changes or redeployment.

## Alternatives Considered:
*Hardcoded Values*
Rejected because every policy update would require developer involvement, code modification, testing, and redeployment.

## Consequences:
*Positive:*
* Improved maintainability.
* Supports that the  policy and autonomy thresholds must be externally configurable
* Finance can update policies independently.
* Faster policy changes.
*Negative:*
* Configuration validation is required.
* Incorrect configuration may affect system behavior.