# ADR-001 - AI Autonomy Policy

## Status:
Accepted

## Context:
The system processes real financial transactions.
The company policy decides
on the approval rules.
LLM outputs are probabilistic rather than deterministic, while financial decisions must be deterministic, auditable, and follow business rules with business rules.
The system must guarantee that company policy is enforced regardless of the AI recommendation.

## Decision:
The system adopts a *Risk-Averse Autonomy Model.*
The *Approval Service* acts as the *Policy Enforcement Point.* It validates every AI recommendation against the configured business policy and is the only component allowed to make the final approval decision.
The AI provides recommendations only.

Automatic approval is allowed only when *all* of the following conditions are satisfied:
* Invoice amount is below the configured approval threshold.
* Expense category allows autonomous approval.
* Vendor validation succeeds.
* No duplicate submission is detected.
* No risk flags are returned.
* AI confidence is above the configured threshold.

Otherwise, the request is escalated to a human approver.

## Alternatives Considered:
*Fully Autonomous AI*
Rejected because it cannot guarantee that the system will be provably incapable of auto-approving above the configured ceiling and creates too much financial risk.

*Human Approval for Every Invoice*
Rejected because it removes most of the automation benefits and increases manual workload.

## Consequences:
*Positive:*
* Reduced business risk.
* Explainable approval decisions.
* Full auditability.
* Easier compliance with business rules.
* Guarantees compliance with M12.
*Negative:*
* Lower automation rate.
* More manual approvals.
* Lower cost savings.