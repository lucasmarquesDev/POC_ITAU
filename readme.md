Aqui está o `README.md` completo, incluindo a explicação do CI/CD configurado para deploy no Amazon ECR:

---

# POC ITAU - Seviço producer

Este projeto é uma POC um serviço producer de notificações utilizando **Kafka** para mensageria, **MediatR** para implementação do padrão CQRS, **Polly** para resiliência, **OpenTelemetry** para observabilidade e **BenchmarkDotNet** para medição de desempenho.

## Tecnologias Utilizadas

- **.NET**: Plataforma de desenvolvimento.
- **Kafka**: Sistema de mensageria distribuída.
- **MediatR**: Biblioteca para implementação do padrão CQRS.
- **Polly**: Biblioteca para políticas de resiliência (retry, circuit breaker, fallback).
- **OpenTelemetry**: Ferramenta para rastreamento e métricas.
- **BenchmarkDotNet**: Biblioteca para benchmarking de desempenho.
- **Datadog**: Plataforma de observabilidade e monitoramento.
- **Xunit**: Framework para testes unitários e de integração.

## Estrutura do Projeto

O projeto está organizado da seguinte forma:

1. **POC_ITAU.API**:
   - Contém os controladores e middlewares da aplicação.
   - Configuração do OpenTelemetry e logging com Serilog.
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
   - Utiliza OpenTelemetry para rastreamento e métricas.
   - Integra com Datadog para monitoramento centralizado.

3. **Benchmarking**:
   - Mede o desempenho do caso de uso de criação de notificações.

4. **Resiliência**:
   - Implementa retry, circuit breaker e fallback para garantir a confiabilidade do sistema.

## Como Executar o Projeto

### Pré-requisitos

- [.NET SDK](https://dotnet.microsoft.com/download) instalado.
- [Docker](https://www.docker.com/) para rodar o Kafka localmente.

## CI/CD - Deploy para Amazon ECR

O projeto inclui um pipeline de CI/CD configurado no GitHub Actions para fazer o deploy da aplicação no **Amazon Elastic Container Registry (ECR)**. O pipeline é acionado sempre que há um push para a branch `main`.

### Passos do Pipeline

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