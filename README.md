# 🚀 Microservices E-Commerce Backend (.NET 8)

[![.NET Version](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![C# Version](https://img.shields.io/badge/C%23-12.0-239120?logo=c-sharp)](https://learn.microsoft.com/en-us/dotnet/csharp/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![Microservices](https://img.shields.io/badge/Architecture-Microservices-%2300C7B7)](https://learn.microsoft.com/en-us/dotnet/architecture/microservices/)

Enterprise-grade e-commerce backend system built with .NET 8, implementing microservices architecture patterns and cloud-native principles.

![System Architecture](https://via.placeholder.com/800x400.png?text=.NET+Microservices+Architecture+Diagram) *Replace with actual diagram from your portfolio*

## 🔧 Core Services

| Service          | Technology Stack             | Description                                  |
|------------------|-------------------------------|----------------------------------------------|
| **Product API**  | EF Core + SQL Server          | Product catalog management                   |
| **Cart API**     | Redis + ASP.NET Core          | Real-time shopping cart service              |
| **Order API**    | Dapper + PostgreSQL           | Order processing with Saga pattern           |
| **Identity API** | IdentityServer + JWT          | Secure authentication/authorization          |
| **API Gateway**  | Ocelot                        | Unified entry point with rate limiting       |
| **Email Service**| MailKit + RabbitMQ            | Event-driven notifications                   |

## 🛠️ Tech Stack

**Core Technologies:**
- **Framework**: .NET 8 + ASP.NET Core
- **Database**: SQL Server, PostgreSQL, Redis
- **Messaging**: RabbitMQ with MassTransit
- **API Gateway**: Ocelot
- **Testing**: xUnit, Moq, TestContainers
- **Observability**: OpenTelemetry, Serilog
- **Containerization**: Docker, Kubernetes

**Architecture Patterns:**
- Clean Architecture
- CQRS with MediatR
- Event-Driven Architecture
- Circuit Breaker (Polly)
- Distributed Tracing
- Health Checks API

## 🚀 Getting Started

### Prerequisites
- .NET 8 SDK
- Docker Desktop
- SQL Server 2022
- RabbitMQ 3.12+

### Installation

1. **Clone the repository**
```bash
git clone https://github.com/youssef130817/MicrocervicesEcommerceBackEnd.git
cd MicrocervicesEcommerceBackEnd
