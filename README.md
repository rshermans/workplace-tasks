# 🏢 WorkPlace Tasks — Fullstack RBAC Management

A premium, methodic implementation of a WorkPlace Management system. Built with **.NET 8**, **PostgreSQL**, and **React**, following industry best practices like Layered Architecture, Clean Code, and robust Role-Based Access Control (RBAC).

---

## ✨ Features

- **🔐 Robust Security**: Role-Based Access Control (RBAC) enforced at the API level.
- **🏗️ Solid Architecture**: Clean separation of concerns (API -> Service -> Repository).
- **🚀 Real-time Feedback**: Modern React UI with immediate RBAC-aware visual states.
- **📄 API Documentation**: Fully documented with Swagger/OpenAPI.
- **🧪 High Testability**: Unit and integration tests for core business rules.
- **🗄️ Database Ready**: PostgreSQL integration with automatic migrations and seeding.

---

## 🛠️ Technology Stack

| Layer | Technology |
| :--- | :--- |
| **Backend** | .NET 8, EF Core, Npgsql, JWT |
| **Frontend** | React 18, Vite, TypeScript, Axios |
| **Database** | PostgreSQL |
| **DevOps** | Docker, Docker Compose |

---

## 🚀 Quick Start

### ⚡ The Easy Way (Automated Script)
**Recomendado para desenvolvimento:**

```bash
# Iniciar aplicação (com limpeza automática de processos anteriores)
.\start.bat

# Parar aplicação (terminar todos os serviços)
.\stop.bat
```

O script `start.bat` faz:
1. ✅ Limpeza de processos anteriores (dotnet.exe, node.exe)
2. ✅ Verificação de requisitos (.NET SDK, Node.js)
3. ✅ Inicialização do PostgreSQL (via Docker)
4. ✅ Build do backend e aplicação de migrações
5. ✅ Instalação de dependências do frontend (se necessário)
6. ✅ Abertura de duas janelas separadas (Backend + Frontend)

### 🐳 Docker Compose (Alternativa)
```bash
# Start everything (DB + Services)
docker compose up -d
```

### 👨‍💻 Manual Startup
*(Ver [Getting Started Guide](docs/GETTING_STARTED.md) para detalhes de inicialização manual)*

---

## 🗄️ Database Setup (Novo!)

Este projeto suporta **3 opções de base de dados**. Escolha a que melhor se adequa:

### ⚡ Opção 1: Supabase Online (Recomendado para Demos)
**Vantagens**: Clone e funciona imediatamente, sem configuração local.

1. Copie `.env.example` para `.env` no diretório `backend/WorkplaceTasks.Api/` (ou crie na raiz).
2. Descomente a linha do Supabase.
3. Execute `.\start.bat`

```bash
# .env
DATABASE_URL="User Id=postgres...;Server=aws-1-eu-west-2.pooler.supabase.com..."
```

### 🐘 Opção 2: PostgreSQL Local
**Vantagens**: Controlo total, desenvolvimento offline.

1. Copie `.env.example` para `.env`
2. Descomente a linha do PostgreSQL local
3. Ajuste username/password se necessário

```bash
# .env
DATABASE_URL="Host=localhost;Port=5432;Database=workplace_tasks..."
```

### 🪶 Opção 3: SQLite (Fallback Automático)
**Vantagens**: Zero configuração. Se nenhuma variável for definida, o SQLite é usado automaticamente.

```bash
# .env
DATABASE_URL="Data Source=workplace_tasks.db"
```

> 🔐 **Segurança**: As credenciais são carregadas do ficheiro `.env` (que é ignorado pelo Git). O ficheiro `appsettings.json` contém apenas configurações padrão seguras.

---


## 🔐 Role-Based Access Control (RBAC)

The system is pre-seeded with three tiers of users for testing:

| Role | Permissions | Test Account |
| :--- | :--- | :--- |
| **Admin** | Full system control. Can view/edit/delete any task. | `admin@example.com` / `Password123!` |
| **Manager** | View all tasks. Can edit any. Can **only delete** own tasks. | `manager@example.com` / `Password123!` |
| **Member** | Create tasks. View own. Edit **only own** tasks. | `member@example.com` / `Password123!` |

> [!IMPORTANT]
> Security is enforced at the **Backend Layer**. The frontend UI only reflects these permissions for better UX (hiding buttons the user cannot use).

### 🧪 Como Testar RBAC

Para validar o sistema de permissões:

1. **Aceda à aplicação**: http://localhost:5173
2. **Faça login com cada role**:
   - Admin: `admin@example.com` / `Password123!`
   - Manager: `manager@example.com` / `Password123!`
   - Member: `member@example.com` / `Password123!`
3. **Verifique as permissões**:
   - **Admin** pode ver/editar/apagar TODAS as tarefas
   - **Manager** pode ver/editar todas, mas só apaga as suas
   - **Member** só vê/edita/apaga as suas próprias tarefas

> [!TIP]
> Para testes automatizados de RBAC, execute: `cd backend && dotnet test`

---

## 💡 Decisões Técnicas

Este projeto segue princípios de **Clean Architecture** e **SOLID**:

- **Separação de Responsabilidades**: Controllers → Services → Repositories
- **RBAC Centralizado**: Policies no Domain Layer, nunca no frontend
- **Global Error Handling**: Middleware para respostas consistentes
- **Database Flexibility**: Suporte para PostgreSQL, Supabase, e SQLite

📖 **Detalhes completos**: [ARCHITECTURE.md](ARCHITECTURE.md)

---

## 📚 Detailed Documentation

Dive deeper into our technical decisions and guides:

- 🏗️ **[Architecture Decisions](ARCHITECTURE.md)**: Deep dive into the monorepo and layered design.
- 🛡️ **[RBAC Test Matrix](docs/rbac-test-matrix.md)**: Detailed breakdown of permissions.
- ⚠️ **[API Error Handling](docs/api-errors.md)**: Consistent response formats.
- 🧪 **[Testing Strategy](TESTING.md)**: How we ensure quality.
- 📜 **[API Contract](API-CONTRACT.md)**: Detailed API request/response formats.
- 📖 **[User Manual](docs/manual-utilizador.md)**: Guide for end-users.

---

## 🔮 Pontos de Melhoria (Se Houvesse Mais Tempo)

### Backend
- [ ] **Paginação Avançada**: Implementar cursor-based pagination para grandes volumes
- [ ] **Soft Delete**: Manter histórico de tarefas apagadas
- [ ] **Audit Logging**: Rastrear todas as alterações (quem/quando/o quê)
- [ ] **Rate Limiting**: Proteção contra abuso de API
- [ ] **Caching**: Redis para queries frequentes

### Frontend
- [ ] **Filtros Avançados**: Por status, data, utilizador atribuído
- [ ] **Drag & Drop**: Reordenação de tarefas
- [ ] **Notificações Real-time**: WebSockets para updates instantâneos
- [ ] **Dark Mode**: Tema escuro completo
- [ ] **Testes E2E**: Playwright para fluxos críticos

### DevOps
- [ ] **CI/CD Pipeline**: GitHub Actions para deploy automático
- [ ] **Docker Multi-stage**: Builds otimizadas
- [ ] **Monitoring**: Application Insights ou Sentry
- [ ] **Load Testing**: k6 para validar performance sob carga

---

Built with pride for the ndBIM technical challenge.
