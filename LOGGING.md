# Sistema de Logging - CSIFLEX Server

## Visão Geral

O CSIFLEX Server utiliza **Serilog** como framework de logging, proporcionando logs estruturados, configuráveis e de alto desempenho em toda a aplicação.

---

## Configuração

### Localização dos Logs

Os logs são gravados em duas localizações:

1. **Console** - Saída padrão durante execução
2. **Arquivos** - Pasta `logs/` no diretório da aplicação

```
CSIFlex.Web/
└── logs/
    ├── csiflex-20260205.log         # Logs gerais
    ├── csiflex-20260206.log
    └── errors/
        ├── csiflex-error-20260205.log  # Apenas erros
        └── csiflex-error-20260206.log
```

### Níveis de Log

O sistema utiliza os seguintes níveis de log:

| Nível | Descrição | Uso |
|:------|:----------|:----|
| **Debug** | Informações detalhadas para diagnóstico | Desenvolvimento e troubleshooting |
| **Information** | Eventos normais da aplicação | Operações bem-sucedidas |
| **Warning** | Situações inesperadas mas não críticas | Credenciais inválidas, dados faltando |
| **Error** | Erros que impedem uma operação | Falhas de conexão, exceções |
| **Critical** | Falhas graves que podem derrubar a aplicação | Erros fatais, corrupção de dados |

---

## Configuração no appsettings.json

### Produção (appsettings.json)

```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Debug",
      "Override": {
        "Microsoft": "Information",
        "Microsoft.AspNetCore": "Warning",
        "System": "Information"
      }
    },
    "WriteTo": [
      {
        "Name": "Console"
      },
      {
        "Name": "File",
        "Args": {
          "path": "logs/csiflex-.log",
          "rollingInterval": "Day",
          "retainedFileCountLimit": 30,
          "fileSizeLimitBytes": 10485760,
          "rollOnFileSizeLimit": true
        }
      },
      {
        "Name": "File",
        "Args": {
          "path": "logs/errors/csiflex-error-.log",
          "rollingInterval": "Day",
          "retainedFileCountLimit": 90,
          "restrictedToMinimumLevel": "Error"
        }
      }
    ],
    "Enrich": [ "FromLogContext", "WithMachineName", "WithThreadId" ]
  }
}
```

### Desenvolvimento (appsettings.Development.json)

```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Debug",
      "Override": {
        "Microsoft": "Debug",
        "Microsoft.AspNetCore": "Information",
        "System": "Debug"
      }
    }
  }
}
```

---

## Política de Retenção

### Logs Gerais
- **Rotação:** Diária
- **Retenção:** 30 dias
- **Tamanho máximo:** 10 MB por arquivo
- **Rotação por tamanho:** Sim

### Logs de Erro
- **Rotação:** Diária
- **Retenção:** 90 dias (3 meses)
- **Nível mínimo:** Error
- **Tamanho:** Sem limite

---

## Formato dos Logs

### Template Padrão

```
[2026-02-05 18:30:45.123 -03:00] [INF] [CSIFlex.Application.Services.AuthenticationService] Login realizado com sucesso para o usuário: admin (Tipo: admin)
```

### Componentes

- **Timestamp:** Data e hora com milissegundos e timezone
- **Nível:** Debug (DBG), Information (INF), Warning (WRN), Error (ERR), Critical (CRT)
- **Contexto:** Namespace completo da classe
- **Mensagem:** Descrição do evento
- **Exceção:** Stack trace completo (quando aplicável)

---

## Logging por Camada

### 1. Program.cs (Inicialização)

```csharp
Log.Information("===========================================");
Log.Information("Iniciando CSIFLEX Server Application");
Log.Information("===========================================");
```

**Eventos registrados:**
- Inicialização da aplicação
- Configuração de serviços
- Configuração do pipeline HTTP
- Ambiente detectado (Development/Production)
- URLs de escuta
- Encerramento da aplicação
- Erros fatais

### 2. AuthenticationService (Aplicação)

```csharp
_logger.LogInformation("Tentativa de login para o usuário: {UserName}", loginDto.UserName);
_logger.LogWarning("Falha no login - Credenciais inválidas para o usuário: {UserName}", loginDto.UserName);
_logger.LogError(ex, "Erro crítico durante o processo de login para o usuário: {UserName}", loginDto.UserName);
```

**Eventos registrados:**
- Tentativas de autenticação
- Validação de credenciais
- Verificação de permissões
- Geração de hash de senha
- Sucessos e falhas de login

### 3. UserRepository (Infraestrutura)

```csharp
_logger.LogDebug("Buscando usuário por nome: {UserName}", userName);
_logger.LogError(ex, "Erro ao buscar usuário: {UserName}", userName);
```

**Eventos registrados:**
- Operações de leitura/escrita no banco
- Usuários encontrados/não encontrados
- Erros de consulta SQL

### 4. DatabaseContext (Infraestrutura)

```csharp
_logger.LogDebug("Criando conexão assíncrona com o banco de dados");
_logger.LogError(ex, "Erro ao conectar ao banco de dados");
```

**Eventos registrados:**
- Criação de conexões
- Abertura de conexões
- Erros de conexão
- Connection string não encontrada

### 5. Requisições HTTP

```csharp
app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} respondeu {StatusCode} em {Elapsed:0.0000} ms";
});
```

**Eventos registrados:**
- Método HTTP (GET, POST, etc.)
- Caminho da requisição
- Status code da resposta
- Tempo de resposta em milissegundos

---

## Exemplos de Logs

### Login Bem-Sucedido

```
[2026-02-05 18:30:45.123 -03:00] [INF] [CSIFlex.Application.Services.AuthenticationService] Tentativa de login para o usuário: admin
[2026-02-05 18:30:45.145 -03:00] [DBG] [CSIFlex.Infrastructure.Data.DatabaseContext] Criando conexão assíncrona com o banco de dados
[2026-02-05 18:30:45.167 -03:00] [DBG] [CSIFlex.Infrastructure.Data.DatabaseContext] Conexão com o banco de dados estabelecida com sucesso
[2026-02-05 18:30:45.189 -03:00] [DBG] [CSIFlex.Infrastructure.Repositories.UserRepository] Buscando usuário por nome: admin
[2026-02-05 18:30:45.234 -03:00] [DBG] [CSIFlex.Infrastructure.Repositories.UserRepository] Usuário encontrado: admin
[2026-02-05 18:30:45.267 -03:00] [INF] [CSIFlex.Application.Services.AuthenticationService] Usuário autenticado com sucesso: admin
[2026-02-05 18:30:45.289 -03:00] [INF] [CSIFlex.Application.Services.AuthenticationService] Login realizado com sucesso para o usuário: admin (Tipo: admin)
```

### Login Falhou (Credenciais Inválidas)

```
[2026-02-05 18:32:10.456 -03:00] [INF] [CSIFlex.Application.Services.AuthenticationService] Tentativa de login para o usuário: teste
[2026-02-05 18:32:10.478 -03:00] [DBG] [CSIFlex.Infrastructure.Repositories.UserRepository] Buscando usuário por nome: teste
[2026-02-05 18:32:10.501 -03:00] [DBG] [CSIFlex.Infrastructure.Repositories.UserRepository] Usuário não encontrado: teste
[2026-02-05 18:32:10.523 -03:00] [WRN] [CSIFlex.Application.Services.AuthenticationService] Usuário não encontrado: teste
[2026-02-05 18:32:10.545 -03:00] [WRN] [CSIFlex.Application.Services.AuthenticationService] Falha no login - Credenciais inválidas para o usuário: teste
```

### Erro de Conexão com Banco de Dados

```
[2026-02-05 18:35:20.789 -03:00] [DBG] [CSIFlex.Infrastructure.Data.DatabaseContext] Criando conexão assíncrona com o banco de dados
[2026-02-05 18:35:23.012 -03:00] [ERR] [CSIFlex.Infrastructure.Data.DatabaseContext] Erro ao conectar ao banco de dados
MySql.Data.MySqlClient.MySqlException (0x80004005): Unable to connect to any of the specified MySQL hosts.
   at MySql.Data.MySqlClient.NativeDriver.Open()
   at MySql.Data.MySqlClient.Driver.Open()
   at MySql.Data.MySqlClient.Driver.Create(MySqlConnectionStringBuilder settings)
   at MySql.Data.MySqlClient.MySqlPool.CreateNewPooledConnection()
   ...
```

---

## Visualização de Logs

### Console (Desenvolvimento)

Durante o desenvolvimento, os logs são exibidos no console com cores:

```bash
dotnet run
```

### Arquivos (Produção)

Para visualizar logs em produção:

```bash
# Ver logs em tempo real
tail -f logs/csiflex-20260205.log

# Ver apenas erros
tail -f logs/errors/csiflex-error-20260205.log

# Buscar por usuário específico
grep "admin" logs/csiflex-20260205.log

# Buscar por erros
grep "ERR" logs/csiflex-20260205.log
```

### Ferramentas de Análise

Para análise avançada de logs, considere:

- **Seq** - Visualizador de logs estruturados (https://datalust.co/seq)
- **Kibana** - Análise e visualização de logs
- **Grafana Loki** - Agregação de logs
- **Papertrail** - Gerenciamento de logs na nuvem

---

## Melhores Práticas

### 1. Use Níveis Apropriados

```csharp
// ✅ Correto
_logger.LogDebug("Iniciando operação X");
_logger.LogInformation("Operação X concluída com sucesso");
_logger.LogWarning("Operação X demorou mais que o esperado");
_logger.LogError(ex, "Falha ao executar operação X");

// ❌ Incorreto
_logger.LogInformation("Valor da variável: {Value}", value); // Use Debug
_logger.LogError("Usuário não encontrado"); // Use Warning
```

### 2. Use Structured Logging

```csharp
// ✅ Correto - Structured
_logger.LogInformation("Usuário {UserName} fez login às {LoginTime}", userName, DateTime.Now);

// ❌ Incorreto - String interpolation
_logger.LogInformation($"Usuário {userName} fez login às {DateTime.Now}");
```

### 3. Não Logue Informações Sensíveis

```csharp
// ❌ NUNCA faça isso
_logger.LogDebug("Senha do usuário: {Password}", password);
_logger.LogInformation("Token: {Token}", authToken);

// ✅ Correto
_logger.LogDebug("Senha verificada para o usuário: {UserName}", userName);
_logger.LogInformation("Token gerado para o usuário: {UserName}", userName);
```

### 4. Capture Exceções

```csharp
// ✅ Correto
try
{
    // código
}
catch (Exception ex)
{
    _logger.LogError(ex, "Erro ao processar requisição");
    throw;
}

// ❌ Incorreto
catch (Exception ex)
{
    _logger.LogError("Erro: " + ex.Message); // Perde stack trace
}
```

---

## Troubleshooting

### Logs não estão sendo gravados

1. Verifique se a pasta `logs/` existe
2. Verifique permissões de escrita
3. Verifique configuração no `appsettings.json`
4. Verifique se o Serilog está configurado no `Program.cs`

### Logs muito verbosos

Ajuste o nível mínimo no `appsettings.json`:

```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information"  // Era "Debug"
    }
  }
}
```

### Logs ocupando muito espaço

Ajuste a retenção e tamanho:

```json
{
  "Serilog": {
    "WriteTo": [
      {
        "Name": "File",
        "Args": {
          "retainedFileCountLimit": 7,  // Apenas 7 dias
          "fileSizeLimitBytes": 5242880  // 5 MB
        }
      }
    ]
  }
}
```

---

## Monitoramento

### Alertas Recomendados

Configure alertas para:

1. **Erros críticos** - Qualquer log de nível Critical
2. **Taxa de erro** - Mais de 10 erros por minuto
3. **Falhas de conexão** - Erros de conexão com banco de dados
4. **Falhas de autenticação** - Múltiplas tentativas de login falhadas

### Métricas Importantes

Monitore:

- Taxa de requisições por segundo
- Tempo médio de resposta
- Taxa de sucesso/falha de login
- Erros de conexão com banco de dados
- Uso de memória e CPU

---

**© 2026 CSIFLEX - Sistema de Monitoramento Industrial**  
**Versão 2.0.0 - .NET 8 Blazor**
