# .NET Interview Stack

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)
[![Status](https://img.shields.io/badge/Status-In%20Development-orange)]()

A production-grade .NET 8 starter demonstrating modern architecture patterns and engineering practices for senior-level backend roles.

Built in public as a learning project and portfolio piece  exploring **Vertical Slice Architecture**, **CQRS**, **production-ready testing**, and (in later phases) **AI/LLM integration**.

> 🚧 **Status:** Milestone 1 in progress — Foundation (target: 6 weeks)

---

## Why this exists

This repo is a deliberate learning project to:

- Solidify senior-level .NET architecture knowledge (VSA, CQRS, testing strategy)
- Build a portfolio piece that demonstrates production patterns end-to-end
- Document architecture decisions and trade-offs publicly
- Explore AI/LLM integration in the .NET ecosystem

Each completed milestone ships with a write-up on [bardh.dev](https://bardh.dev) covering the decisions, trade-offs, and lessons learned.

---

## Stack

| Layer | Technology |
|---|---|
| Runtime | .NET 8 (LTS) |
| API | ASP.NET Core Web API |
| Architecture | Vertical Slice Architecture |
| Database | PostgreSQL |
| ORM | EF Core 8 |
| Mediator / CQRS | MediatR |
| Validation | FluentValidation (pipeline behaviors) |
| Auth | JWT + refresh tokens |
| Unit testing | xUnit + FluentAssertions |
| Integration testing | WebApplicationFactory + Testcontainers |
| Containerization | Docker + docker-compose |
| CI/CD | GitHub Actions |

---

## Roadmap

### 🚧 Milestone 1 — Foundation *(in progress)*
Core architecture, authentication, validation, testing, containerization.

- [ ] Solution structure (Vertical Slice)
- [ ] EF Core + PostgreSQL setup with migrations
- [ ] MediatR pipeline behaviors (logging, validation)
- [ ] FluentValidation integration
- [ ] JWT authentication + refresh tokens
- [ ] Global error handling (Problem Details, RFC 7807)
- [ ] API versioning + OpenAPI
- [ ] Unit + integration test suite (Testcontainers)
- [ ] Dockerfile + docker-compose
- [ ] GitHub Actions CI

### ⏳ Milestone 2 — Production Patterns
Background jobs, caching, observability, rate limiting.

- [ ] Hangfire for background processing
- [ ] Redis (distributed cache + rate limiting)
- [ ] File storage abstraction (Local / Azure Blob / S3)
- [ ] Email service (SMTP + Razor templates)
- [ ] Serilog structured logging
- [ ] Health checks (liveness + readiness)

### ⏳ Milestone 3 — AI Integration
LLM abstraction, streaming, vector DB, MCP server.

- [ ] Multi-provider LLM abstraction (OpenAI / Anthropic / Groq / OpenRouter)
- [ ] Streaming responses (Server-Sent Events)
- [ ] Token usage + cost tracking
- [ ] Vector DB integration (pgvector)
- [ ] MCP server scaffold
- [ ] RAG demo (document upload → semantic search)

---

## Getting started

> Setup instructions will be added as Milestone 1 progresses.

```bash
# Prerequisites: .NET 8 SDK, Docker, Docker Compose

git clone https://github.com/hoxhajbardhi/dotnet-interview-stack.git
cd dotnet-interview-stack

# Coming soon
docker-compose up -d
dotnet run --project src/Api
```

---

## Architecture decisions

Major decisions are documented as the project evolves. Key choices so far:

- **Vertical Slice Architecture** over traditional Clean / Onion — better feature locality, less ceremony, easier to navigate as the codebase grows.
- **PostgreSQL** over SQL Server — open, performant, widely deployed in modern .NET stacks.
- **Testcontainers** for integration tests — real database, no mocks, closer to production.

Detailed write-ups will be linked here after each milestone.

---

## Write-ups

Long-form articles published on [bardh.dev](https://bardh.dev):

- *Coming after Milestone 1:* Building a Modern .NET API — Architecture Decisions & Patterns
- *Coming after Milestone 2:* Production-Ready .NET — Caching, Background Jobs & Observability
- *Coming after Milestone 3:* Adding AI Superpowers to .NET — LLMs, RAG & MCP

---

## License

MIT — see [LICENSE](LICENSE). Use it however you want.

---

## About

Built by **Fatbardh Hoxhaj** — Senior .NET consultant

- Website: [bardh.dev](https://bardh.dev)
- GitHub: [@hoxhajbardhi](https://github.com/hoxhajbardhi)
- LinkedIn: *https://www.linkedin.com/in/fatbardhhoxhaj/*
