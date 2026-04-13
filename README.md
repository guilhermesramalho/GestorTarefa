# GestorTarefa

Projeto exemplo de gerenciamento de tarefas seguindo boas práticas (arquitetura em camadas). 

Tecnologias:
- .NET 9
- Entity Framework Core
- PostgreSQL
- Docker / docker-compose
- Swashbuckle (Swagger)

Como rodar:
1. Configure Docker e Docker Compose
2. Rode `docker-compose up --build`
3. A aplicação estará disponível em `http://localhost:5000`
4. Abra Swagger em `http://localhost:5000/swagger`

Rodar migrations:
- Entre no container da aplicação ou rode dotnet ef migrations add Inicial e dotnet ef database update apontando para a connection string do docker.

Exemplos de requisições (JSON):
- POST /tasks
{
  "title": "Nova tarefa",
  "description": "Descrição",
  "status": 0,
  "priority": 1,
  "responsible": "Fulano",
  "dueDate": "2026-01-01T00:00:00"
}

Observações:
- O projeto é um ponto de partida e pode ser expandido com autenticação, validação mais robusta, testes, etc.

Migrations e Seed (automático):
- Ao subir via `docker-compose up --build`, a aplicação tentará aplicar automaticamente as migrations usando `Database.Migrate()`.
- A inicialização aguarda o banco PostgreSQL ficar disponível (retry simples). Após as migrations, o projeto executa um seeding idempotente que insere 100 tarefas caso a tabela esteja vazia.

Comandos rápidos:
- `docker-compose up --build` — sobe postgres e aplicação; migrations e seed serão aplicados automaticamente.
- Swagger: `http://localhost:5000/swagger`

Criar novas migrations (desenvolvimento):

1. Instale a ferramenta `dotnet-ef` globalmente (se ainda não estiver instalada):

   `dotnet tool install --global dotnet-ef`

2. No diretório do projeto, crie a migration:

   `dotnet ef migrations add NomeDaMigration`

3. Commit os arquivos gerados de migrations no repositório (migrations devem ser versionadas).

Observação:
- Não crie migrations em runtime dentro do container. A aplicação no container apenas aplica (`Database.Migrate()`), não cria migrations.

CI (GitHub Actions):
- Foi adicionado o workflow `/.github/workflows/ci.yml` que executa build, testes e gera a imagem Docker (`ghcr.io/<repo>:<sha>`). O pipeline dispara em `push` e `pull_request` para a branch `develop`.

Power BI:
- Na pasta `PowerBI` existe o arquivo `controleTarefas.pbix` (arquivo para importação no Power BI). Contém um dashboard simples para visualização de atividades e métricas de tarefas.
