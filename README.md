# IBGE Stats API

API para consultar notícias do IBGE com sistema de autenticação e favoritos.

## Tecnologias

- .NET 8.0 Web API
- Entity Framework Core + SQL Server
- JWT Authentication
- Swagger/OpenAPI
- BCrypt for passwords hash

## Run

### 1. Prerequisites
- .NET 8.0 SDK
- SQL Server

### 2. Settings

Clone the project:
```bash
git clone https://github.com/AlanisOliveira/IBGE-Stats-API.git
cd IbgeStats
```

Configure a connection string no `appsettings.Development.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=IbgeStatsDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=true"
  }
}
```

### 3. Banco de dados

```bash
dotnet ef database update
```

### 4. Executar

```bash
dotnet run
```

A API vai estar disponível em:
- HTTP: http://localhost:5287
- Swagger: https://localhost:5287/swagger

## Endpoints

### Autenticação
- `POST /api/auth/register` - Registrar usuário
- `POST /api/auth/login` - Login (retorna JWT token)
- `GET /api/auth/me` - Dados do usuário logado

### Pesquisas IBGE
- `GET /api/pesquisas` - Lista todas as pesquisas
- `GET /api/pesquisas/{id}` - Detalhes de uma pesquisa
- `GET /api/pesquisas/{id}/indicadores` - Indicadores da pesquisa 
- `GET /api/pesquisas/{id}/periodos` - Períodos da pesquisa
- `POST /api/pesquisas/sync-ibge` - Sincronizar com API do IBGE 

### Favoritos
- `GET /api/favoritos` - Meus favoritos 
- `POST /api/favoritos/{pesquisaId}` - Adicionar favorito 
- `DELETE /api/favoritos/{pesquisaId}` - Remover favorito 
- `GET /api/favoritos/{pesquisaId}/status` - Verificar se é favorito 

**Legenda:**
- Requer autenticação (JWT token)
- Requer role Admin

## Como usar

### 1. Registrar usuário
```json
POST /api/auth/register
{
  "username": "joao",
  "email": "joao@email.com",
  "password": "123456",
  "confirmPassword": "123456"
}
```

### 2. Fazer login
```json
POST /api/auth/login
{
  "email": "joao@email.com", 
  "password": "123456"
}
```

Vai retornar um token JWT. Use esse token nos próximos requests.

### 3. Usar endpoints protegidos

No Swagger: clique em "Authorize" e digite `Bearer SEU_TOKEN`

Ou via Postman/Insomnia: adicione header:
```
Authorization: Bearer SEU_TOKEN
```

### 4. Sincronizar dados do IBGE

Para popular o banco com dados reais do IBGE:
```
POST /api/pesquisas/sync-ibge
```
⚠️ Esse endpoint só funciona para usuários Admin. Por padrão, o primeiro usuário é criado como "User".

## Roles de usuário

- **User**: Pode consultar pesquisas
- **Admin**: Pode sincronizar dados e criar pesquisas

## Estrutura do banco

- **Users**: Usuários da aplicação
- **UserSessions**: Sessões JWT ativas
- **Pesquisas**: Dados das pesquisas do IBGE
- **Indicadores**: Indicadores de cada pesquisa
- **Periodos**: Períodos temporais das pesquisas
- **UserFavorites**: Pesquisas favoritadas por usuário

## Observações

- A API integra diretamente com a API oficial do IBGE: https://servicodados.ibge.gov.br/api/v1/pesquisas
- Senhas são criptografadas com BCrypt
- Tokens JWT expiram em 1 hora

## Problemas conhecidos

- Alguns endpoints da API do IBGE podem estar instáveis
- Sistema de roles ainda é básico (não tem interface para alterar roles)