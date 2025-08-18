# 📦 UsuarioApiRedis

API RESTful em .NET 8 com arquitetura em camadas, utilizando:

- ✅ SQL Server (banco relacional principal)
- ✅ Redis (cache de leitura)
- ✅ RabbitMQ (fila para processamento assíncrono)
- ✅ Docker + Docker Compose (ambiente completo)

---

## 🔧 Tecnologias

- .NET 8
- SQL Server 2022
- Redis 7
- RabbitMQ 3 (com painel de gerenciamento)
- Entity Framework Core
- StackExchange.Redis
- RabbitMQ.Client

---

## 🚀 Funcionalidades

- **POST /api/usuarios**  
  → Envia dados para a fila RabbitMQ (cadastro assíncrono)

- **GET /api/usuarios/{id}**  
  → Busca o usuário por ID (usa cache Redis)

---

## 🐳 Subindo o projeto com Docker

```bash
docker-compose up --build
```

## Estrutura

UsuarioApiRedis/
├── Api/ # API REST
├── Application/ # Lógica de negócio
├── Domain/ # Entidades e interfaces
├── Infrastructure/ # EF Core + Redis + Repos
├── Worker/ # Serviço que consome RabbitMQ
├── docker-compose.yml
├── Dockerfile
├── Dockerfile.Worker
├── README.md
└── .gitignore

API: <http://localhost:5000/swagger>

RabbitMQ Admin: <http://localhost:15672>
Login: guest | Senha: guest
