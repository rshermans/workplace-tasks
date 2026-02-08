# Guia de Configuração PostgreSQL Local (pgAdmin4)

Este documento fornece instruções passo-a-passo para configurar PostgreSQL localmente usando pgAdmin4 para o projeto Workplace Tasks.

## Pré-requisitos

- PostgreSQL instalado localmente (versão 12 ou superior recomendada)
- pgAdmin4 instalado e configurado

## Passo 1: Criar Servidor no pgAdmin4

1. Abra o **pgAdmin4**
2. No painel esquerdo, clique com botão direito em **Servers** → **Register** → **Server**
3. Na aba **General**:
   - **Name**: `WorkplaceTasks-Local` (ou nome de sua preferência)
4. Na aba **Connection**:
   - **Host name/address**: `localhost`
   - **Port**: `5432` (porta padrão do PostgreSQL)
   - **Maintenance database**: `postgres`
   - **Username**: `postgres` (ou seu usuário PostgreSQL)
   - **Password**: [Sua senha do PostgreSQL]
   - ✅ Marque **Save password** (opcional, para conveniência)
5. Clique em **Save**

## Passo 2: Criar Base de Dados

1. No painel esquerdo, expanda o servidor que acabou de criar
2. Clique com botão direito em **Databases** → **Create** → **Database**
3. Na aba **General**:
   - **Database**: `workplace_tasks`
   - **Owner**: `postgres` (ou seu usuário)
   - **Encoding**: `UTF8`
4. Clique em **Save**

## Passo 3: Verificar Conexão

Execute o seguinte SQL para testar a conexão:

1. Clique com botão direito na database `workplace_tasks` → **Query Tool**
2. Execute:
```sql
SELECT version();
```
3. Se retornar a versão do PostgreSQL, a conexão está funcionando ✅

## Passo 4: Atualizar Connection String no Projeto

Edite o arquivo `backend/WorkplaceTasks.Api/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=workplace_tasks;Username=postgres;Password=SUA_SENHA_AQUI"
  }
}
```

> ⚠️ **Importante**: Substitua `SUA_SENHA_AQUI` pela senha real do PostgreSQL.

## Passo 5: Reverter para Npgsql no Código

No arquivo `backend/WorkplaceTasks.Api/Program.cs`, altere:

```csharp
// De:
options.UseSqlite(connectionString)

// Para:
options.UseNpgsql(connectionString)
```

## Passo 6: Recriar Migrations

Execute os seguintes comandos no terminal (na raiz do projeto):

```bash
# Remover migrations antigas do SQLite
rm -r backend/WorkplaceTasks.Api/Infrastructure/Migrations

# Criar nova migration para PostgreSQL
dotnet ef migrations add InitialCreate -o Infrastructure/Migrations --project backend/WorkplaceTasks.Api/WorkplaceTasks.Api.csproj

# Aplicar migration ao banco
dotnet ef database update --project backend/WorkplaceTasks.Api/WorkplaceTasks.Api.csproj
```

## Passo 7: Verificar Tabelas Criadas

No pgAdmin4:
1. Expanda `workplace_tasks` → **Schemas** → **public** → **Tables**
2. Deve ver as tabelas:
   - `Users`
   - `Tasks`
   - `__EFMigrationsHistory`

## Passo 8: Executar a API

```bash
cd backend
dotnet run --project WorkplaceTasks.Api/WorkplaceTasks.Api.csproj
```

A API executará o **DbSeeder** automaticamente, criando os usuários de teste.

## Troubleshooting

### Erro: "password authentication failed"
- Verifique se a senha no `appsettings.Development.json` está correta
- Confirme que o usuário `postgres` tem permissões

### Erro: "could not connect to server"
- Verifique se o serviço PostgreSQL está rodando:
  - Windows: Services → PostgreSQL
  - Ou execute: `pg_isready -h localhost -p 5432`

### Erro: "database does not exist"
- Certifique-se de ter criado a database `workplace_tasks` no Passo 2

## Credenciais de Teste (Após Seed)

Após a API rodar pela primeira vez, os seguintes usuários estarão disponíveis:

- **Admin**: `admin@example.com` / `Password123!`
- **Manager**: `manager@example.com` / `Password123!`
- **Member**: `member@example.com` / `Password123!`
