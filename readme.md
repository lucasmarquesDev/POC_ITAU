# POC ITAU - Serviço Producer

Este projeto é uma POC de um serviço producer em .NET 8 de notificações utilizando **Kafka** para mensageria, **MediatR** para implementação do padrão CQRS, **Polly** para resiliência, **ElasticSearch** para centralização de métricas, e **BenchmarkDotNet** para medição de desempenho. No padrão arquitetural **Clean Architecture**.

## Tecnologias Utilizadas

- **.NET**: Plataforma de desenvolvimento.
- **Kafka**: Sistema de mensageria distribuída.
- **MediatR**: Biblioteca para implementação do padrão CQRS.
- **Polly**: Biblioteca para políticas de resiliência (retry, circuit breaker, fallback).
- **ElasticSearch**: Ferramenta para coleta e centralização de métricas.
- **BenchmarkDotNet**: Biblioteca para benchmarking de desempenho.
- **Xunit**: Framework para testes unitários e de integração.

## Estrutura do Projeto

O projeto está organizado da seguinte forma:

1. **POC_ITAU.API**:
   - Contém os controladores e middlewares da aplicação.
   - Configuração do logging com Serilog.
   - Configuração com Kafka para envio de notificações.

2. **POC_ITAU.Application**:
   - Contém os casos de uso e handlers do MediatR.
   - Implementação de políticas de resiliência com Polly.
   - Mapeamento de objetos com AutoMapper.

3. **POC_ITAU.Persistence.Kafka**:
   - Implementação do serviço de produção de mensagens no Kafka.

4. **POC_ITAU.BenchmarkPerformance**:
   - Testes de desempenho utilizando BenchmarkDotNet.

5. **POC_ITAU.IntegrationTest**:
   - Testes de integração para validar o fluxo de notificações.

## Funcionalidades Principais

1. **Envio de Notificações**:
   - Recebe uma solicitação de notificação via API.
   - Produz uma mensagem no tópico `email-notifications` do Kafka.
   - Implementa políticas de resiliência para lidar com falhas no Kafka.

2. **Observabilidade**:
   - Utiliza ElasticSearch para centralização de métricas e logs.

3. **Benchmarking**:
   - Mede o desempenho do caso de uso de criação de notificações.

4. **Resiliência**:
   - Implementa retry, circuit breaker e fallback para garantir a confiabilidade do sistema.

## Como Executar o Projeto

### Pré-requisitos

- [.NET SDK](https://dotnet.microsoft.com/download) instalado.
- [Docker](https://www.docker.com/) para rodar o Kafka e o ElasticSearch localmente.

### Rodando com Docker Compose

O projeto inclui um arquivo `docker-compose.yml` para facilitar a execução de todas as dependências (Kafka, ElasticSearch) e a aplicação em contêineres Docker.

#### Passos para Execução:

1. **Clone o repositório**:
   ```bash
   git clone https://github.com/lucasmarquesDev/POC_ITAU.git
   cd POC_ITAU
   ```

2. **Suba os contêineres**:
   Execute o seguinte comando para subir todos os serviços (Kafka, ElasticSearch e a aplicação):
   ```bash
   docker-compose up --build
   ```

   Isso irá:
   - Iniciar o Kafka.
   - Iniciar o ElasticSearch para centralização de logs e métricas.
   - Construir e iniciar a aplicação.

3. **Acesse a aplicação**:
   - A API estará disponível em `http://localhost:8080`.

4. **Envie uma notificação**:
   - Faça uma requisição POST para o endpoint `/notification/sendEmail` com o seguinte corpo:
     ```json
     {
       "destination": "test@example.com",
       "subject": "Test Subject",
       "message": "Test Message"
     }
     ```

5. **Verifique as métricas no ElasticSearch**:
   - Acesse o ElasticSearch para verificar os logs e métricas geradas pela aplicação.

---

### CI/CD - Deploy para Amazon ECR

O projeto inclui um pipeline de CI/CD configurado no GitHub Actions para fazer o deploy da aplicação no **Amazon Elastic Container Registry (ECR)**. O pipeline é acionado sempre que há um push para a branch `main`.

#### Passos do Pipeline

1. **Checkout do código**:
   - O código é clonado do repositório.

2. **Execução dos testes**:
   - Os testes unitários são executados para garantir que o código está funcionando corretamente.

3. **Configuração das credenciais da AWS**:
   - As credenciais da AWS são configuradas usando segredos armazenados no GitHub.

4. **Login no Amazon ECR**:
   - O pipeline faz login no Amazon ECR para permitir o push da imagem Docker.

5. **Build, tag e push da imagem Docker**:
   - A imagem Docker é construída, taggeada com `latest` e enviada para o repositório no ECR.

---

### Estrutura do `docker-compose.yml`

O arquivo `docker-compose.yml` define os seguintes serviços:

1. **Kafka**:
   - Serviço de mensageria distribuída.
   - Expõe a porta `29092` para o host.

2. **ElasticSearch**:
   - Serviço para centralização de logs e métricas.
   - Expõe a porta `9200` para o host.

3. **App**:
   - Aplicação principal.
   - Depende do Kafka e do ElasticSearch.
   - Expõe as portas `8080` e `8081` para o host.

---

### Exemplo de Requisição

Para enviar uma notificação, faça uma requisição POST para o endpoint `/notification/sendEmail`:

**Endpoint**:
```
POST http://localhost:8080/notification/sendEmail
```

**Corpo da Requisição**:
```json
{
  "destination": "test@example.com",
  "subject": "Test Subject",
  "message": "Test Message"
}
```

---

### Contribuição

Contribuições são bem-vindas! 