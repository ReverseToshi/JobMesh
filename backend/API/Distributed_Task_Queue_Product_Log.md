# Product Log – Distributed Task Queue System

| ID | Feature | Priority | Status | Description |
|----|----------|----------|---------|-------------|
| PL-001 | Project Setup | High | Not Started | Create ASP.NET Core solution, Docker setup, Git repository, CI pipeline. |
| PL-002 | PostgreSQL Integration | High | Not Started | Configure EF Core and MySQL connection. |
| PL-003 | Redis Integration | High | Not Started | Configure Redis connection and queue infrastructure. |
| PL-004 | Job Entity | High | Not Started | Create database model for jobs and execution metadata. |
| PL-005 | Job Submission API | High | Not Started | Endpoint to create and enqueue jobs. |
| PL-006 | Job Status API | High | Not Started | Endpoint to retrieve job status and execution details. |
| PL-007 | Queue Service | High | Not Started | Service for adding and retrieving jobs from Redis. |
| PL-008 | Worker Service | High | Not Started | Background service that consumes jobs from Redis. |
| PL-009 | Job State Management | High | Not Started | Track Pending, Running, Completed, Failed states. |
| PL-010 | Job Cancellation | Medium | Not Started | Allow cancellation of jobs before execution. |
| PL-011 | Retry Mechanism | High | Not Started | Retry failed jobs with configurable limits. |
| PL-012 | Dead Letter Queue | Medium | Not Started | Store jobs that exceed retry limits. |
| PL-013 | Worker Registration | Medium | Not Started | Track active worker nodes. |
| PL-014 | Worker Heartbeats | Medium | Not Started | Detect failed or disconnected workers. |
| PL-015 | Job Execution Logs | Medium | Not Started | Store execution logs and error messages. |
| PL-016 | Authentication | Medium | Not Started | JWT authentication for API access. |
| PL-017 | Role-Based Authorization | Low | Not Started | Restrict administrative operations. |
| PL-018 | Health Check Endpoint | Medium | Not Started | API endpoint for monitoring service health. |
| PL-019 | Metrics Collection | Medium | Not Started | Track queue length, throughput, and failures. |
| PL-020 | Structured Logging | Medium | Not Started | Implement Serilog for centralized logging. |
| PL-021 | Docker Compose Setup | High | Not Started | Run API, PostgreSQL, Redis, and workers locally. |
| PL-022 | Unit Tests | Medium | Not Started | Test business logic and queue operations. |
| PL-023 | Integration Tests | Medium | Not Started | Test API and database interactions. |
| PL-024 | API Documentation | Medium | Not Started | Swagger/OpenAPI documentation. |
| PL-025 | Dashboard UI | Low | Not Started | Web dashboard for monitoring jobs and workers. |
| PL-026 | Priority Queues | Medium | Not Started | Support High, Normal, and Low priority jobs. |
| PL-027 | Scheduled Jobs | Low | Not Started | Execute jobs at a future time. |
| PL-028 | Job Dependencies | Low | Not Started | Allow jobs to depend on completion of others. |
| PL-029 | Distributed Locking | Medium | Not Started | Prevent multiple workers processing same job. |
| PL-030 | Kubernetes Deployment | Low | Not Started | Deploy system to Kubernetes cluster. |

---

## MVP Scope (Portfolio Version)

Focus on these first:

- PL-001 Project Setup
- PL-002 PostgreSQL Integration
- PL-003 Redis Integration
- PL-004 Job Entity
- PL-005 Job Submission API
- PL-006 Job Status API
- PL-007 Queue Service
- PL-008 Worker Service
- PL-009 Job State Management
- PL-011 Retry Mechanism
- PL-018 Health Check Endpoint
- PL-021 Docker Compose Setup
- PL-022 Unit Tests
- PL-024 API Documentation

## Skills Demonstrated

- Distributed systems concepts
- ASP.NET Core
- PostgreSQL
- Redis
- Docker
- Background workers
- API development
- Testing and documentation

## Future Enhancements

- Worker heartbeats
- Dead-letter queues
- Metrics and monitoring
- Priority queues
- Kubernetes deployment
