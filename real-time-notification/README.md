# Real-Time Notification System

Este projeto é uma API robusta desenvolvida com **ASP.NET Core 8** focada em notificações em tempo real e monitoramento
de presença de usuários utilizando **SignalR**.

## 🚀 Tecnologias Utilizadas

* **Framework:** .NET 8 (C# 12)
* **Comunicação Real-time:** SignalR
* **Banco de Dados:** PostgreSQL (Entity Framework Core)
* **Segurança:** JWT (JSON Web Token) com integração SignalR
* **Documentação:** Swagger com Versionamento de API
* **Infraestrutura:** Docker & Docker Compose
* **Processamento em Background:** Background Services (Worker) para gerenciamento de presença.

## 🛠️ Arquitetura

O projeto segue uma estrutura organizada em camadas para facilitar a manutenção e escalabilidade:

* **Api:** Contém os Controllers, Hubs do SignalR, Middlewares e Background Services.
* **Application:** Interfaces e serviços de lógica de negócio (JWT, Notificações, Presença).
* **Domain:** Entidades principais e modelos de domínio.
* **Infra:** Contexto do banco de dados (EF Core) e implementações de repositório.

## 📌 Principais Funcionalidades

1. **Notificações em Tempo Real:** Envio de mensagens e alertas instantâneos para usuários conectados via Hub.
2. **Monitoramento de Presença:** Gerenciamento automático do status online/offline dos usuários através do
   `ConnectionHub` e `PresenceWorker`.
3. **Segurança JWT:** Autenticação via token inclusive em conexões WebSockets/SignalR.
4. **Versionamento de API:** Suporte a múltiplas versões da API (v1, v2, etc.) documentadas via Swagger.

## ⚙️ Como Executar o Projeto

### Pré-requisitos

* [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
* [Docker](https://www.docker.com/products/docker-desktop/)

### Passo a Passo

1. **Subir o Banco de Dados (PostgreSQL):**
   ```bash
   docker-compose up -d
   ```

2. **Configurar o Banco de Dados:**
   Certifique-se de que a ConnectionString no `appsettings.json` aponta para o container do Postgres. Em seguida,
   execute as migrações (se necessário):
   ```bash
   dotnet ef database update
   ```

3. **Executar a Aplicação:**
   ```bash
   dotnet run
   ```

4. **Acessar a Documentação:**
   Abra o navegador em `http://localhost:5000/swagger` (ou a porta configurada) para visualizar os endpoints
   disponíveis.

## 🔌 Conexão com o Hub

O Hub de notificações está disponível no endpoint: `/notification-hub`.

Para conectar via cliente SignalR, é necessário enviar o Token JWT via Query String `access_token`, conforme configurado
no `Program.cs`.
