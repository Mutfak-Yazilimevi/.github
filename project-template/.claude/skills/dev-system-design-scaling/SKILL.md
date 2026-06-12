---
name: system-design-scaling
description: Scale a system across growth stages — roughly 1K (early/MVP), 1M (scaling), and 10M+ (enterprise) users. Use when choosing architecture, database, API design, traffic handling, caching, queueing, deployment, observability, fault tolerance, and data-consistency strategies appropriate to the CURRENT scale, or planning the evolution path from MVP to enterprise without premature complexity.
---

# System Design — Scaling by Stage

## Overview

Match design effort to scale. The biggest system-design mistake is building 10M-user
infrastructure for a 1K-user product (premature complexity) — or the reverse, painting
yourself into a corner that can't grow. Pick the **simplest design that fits today** while
keeping a clear evolution path.

## How to use

1. Estimate the **current** scale and 12-month trajectory (users, RPS, data size, read/write mix).
2. Read the row for that stage in the matrix below; adopt those defaults.
3. Only borrow from a higher stage when a **measured** bottleneck justifies it.
4. Re-evaluate per dimension as load grows — scaling is incremental, not a big-bang rewrite.

## Stage matrix

| Aspect | 1K — Early / MVP | 1M — Scaling | 10M+ — Enterprise |
| :--- | :--- | :--- | :--- |
| **Primary goal** | Validate idea fast; ship features | Scale the bottleneck without rewriting | Extreme reliability, scalability, cost-efficiency |
| **Architecture** | Simple monolith / layered | Modular monolith or early services | Fully distributed microservices; clear domain boundaries |
| **API design** | Basic REST/RPC; minimal validation | Versioned APIs, pagination, rate limits, backward compat | API gateway, mature schema management, contracts |
| **Traffic handling** | Direct client→server; one instance | Load balancer across stateless instances | Multi-region, edge/CDN, autoscaling, anycast |
| **Database** | Single relational DB | Read replicas, indexing, connection pooling | Sharding / partitioning; polyglot persistence |
| **Caching** | Little to none | Cache hot reads (Redis/Memcached) | Multi-layer caching (client, CDN, app, DB) |
| **Async / queues** | Synchronous; do work inline | Introduce queues to offload slow work | Event-driven backbone; streaming (Kafka) |
| **Messaging** | Direct calls | Queues for decoupling & retries | Event sourcing where it fits; durable streams |
| **Deployment** | Manual / simple CI/CD | Containerized CI/CD pipelines | Blue-green / canary, automated rollback, IaC |
| **Fault tolerance** | Minimal; accept downtime | Retries, timeouts, health checks | Circuit breakers, bulkheads, graceful degradation |
| **Observability** | Basic logs | Centralized logs + metrics + alerts | Full tracing (distributed), SLOs, anomaly detection |
| **Security** | Auth + basic access control | Token-based auth, RBAC, audit | Zero-trust, secrets management, continuous audit |
| **Data consistency** | Strong (single DB) | Mix of strong + eventual where safe | Deliberate per-flow: strong vs eventual, idempotency |

## Principles that hold at every stage

- **Stateless services** scale horizontally; push state to data stores/caches.
- **Cache the expensive, read-heavy paths** — but plan invalidation before adding cache.
- **Make writes idempotent** before introducing retries, queues, or at-least-once delivery.
- **Measure before scaling** — add capacity/complexity against a real bottleneck, not a guess.
- **Design for failure** — every remote call can time out; degrade, don't collapse.
- **Evolve incrementally** — extract a service or shard a table when load demands it, not before.

## Common evolution path

1. Single DB → add **indexes** → add **read replicas** → **cache** hot reads → **shard** writes.
2. Inline work → **queue** slow tasks → **event-driven** integration between services.
3. Monolith → **modular monolith** → extract the **highest-load bounded context** first.

## Related

- Cloud/infra & cost: `cloud-architect` agent · `performance-engineer` agent.
- DB tuning & query optimization: `database-optimizer` agent.
- Class-level design (below this level): `dev-low-level-design`.
- .NET architecture choice: `dev-dotnet-architecture-selection`.
