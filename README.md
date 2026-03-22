# FGC Catalog API

Microsserviço responsável pelo catálogo de jogos da plataforma **FIAP Cloud Games**.

## Responsabilidades

- CRUD completo de jogos (somente Admin pode criar/editar/deletar)
- Listagem e busca de jogos (usuários autenticados)
- Endpoint de compra que publica `OrderPlacedEvent` no RabbitMQ
- Consumer de `PaymentProcessedEvent`: quando aprovado, adiciona jogo à biblioteca do usuário

## Endpoints

| Método | Rota | Role | Descrição |
|--------|------|------|-----------|
| `POST` | `/games` | Admin | Cadastra novo jogo |
| `GET` | `/games` | User, Admin | Lista todos os jogos |
| `GET` | `/games/{id}` | User, Admin | Busca jogo por ID |
| `PUT` | `/games/{id}` | Admin | Atualiza jogo |
| `DELETE` | `/games/{id}` | Admin | Remove jogo |
| `POST` | `/games/{gameId}/purchase` | User, Admin | Inicia compra do jogo |

### POST /games

```json
{
  "title": "Cyber Quest",
  "description": "RPG futurista",
  "price": 49.90
}
```

Retorna `201 Created` com o `id` do jogo.

### POST /games/{gameId}/purchase

Requer JWT válido. Extrai `userId` do claim, verifica se o jogo existe e publica `OrderPlacedEvent`.

Retorna `202 Accepted` com mensagem de confirmação.

## Eventos

| Tipo | Evento | Quando |
|------|--------|--------|
| Publicado | `OrderPlacedEvent` | Ao iniciar compra |
| Consumido | `PaymentProcessedEvent` | Ao receber resultado do pagamento |

Quando `PaymentProcessedEvent.Status == "Approved"`, o jogo é adicionado à `UserLibrary` do usuário.

## Variáveis de ambiente

| Variável | Descrição | Exemplo |
|----------|-----------|---------|
| `ConnectionStrings__DefaultConnection` | Connection string PostgreSQL | `Host=localhost;Database=catalogdb;...` |
| `Jwt__Key` | Chave secreta JWT (compartilhada com UsersAPI) | `CHAVE_SUPER_SECRETA_MIN_32_CHARS!!` |
| `Jwt__Issuer` | Issuer do token JWT | `FCG.Users.Api` |
| `Jwt__Audience` | Audience do token JWT | `FCG.Users.Api` |
| `RabbitMQ__Host` | Host do RabbitMQ | `localhost` |
| `RabbitMQ__Username` | Usuário RabbitMQ | `guest` |
| `RabbitMQ__Password` | Senha RabbitMQ | `guest` |

## Executar localmente

```bash
dotnet run --project FGC.Catalog.Api
```

Migrações são aplicadas automaticamente na inicialização.

Swagger disponível em `/swagger` no ambiente de desenvolvimento.

## Executar com Docker

```bash
docker compose up --build
```

API disponível em `http://localhost:5001`.

## Arquitetura

```
FGC.Catalog.Api            → Controllers, Middlewares, Program.cs
FGC.Catalog.Application    → Services, DTOs, Contracts/Events
FGC.Catalog.Infrastructure → DbContext, Repositories, Consumers, Migrations
FGC.Catalog.Domain         → Entities (Game, UserLibrary), Repositories interfaces
```
