# CSIFLEX Server - Refatoração .NET 8 Blazor

## Visão Geral

Este projeto é a refatoração do sistema CSIFLEX Server original (VB.NET Windows Forms) para uma arquitetura moderna utilizando **.NET 8**, **Blazor Server** e **Domain-Driven Design (DDD)**.

## Arquitetura

O projeto segue os princípios de **DDD**, **SOLID**, **KISS** e **Clean Code**, organizado em 4 camadas:

### 1. **CSIFlex.Domain** (Camada de Domínio)
Contém as entidades de negócio, interfaces e lógica de domínio pura.

**Estrutura:**
```
CSIFlex.Domain/
├── Entities/
│   └── Authentication/
│       └── User.cs                    # Entidade de usuário
├── Interfaces/
│   ├── Repositories/
│   │   └── IUserRepository.cs         # Interface do repositório
│   └── Services/
│       └── IAuthenticationService.cs  # Interface do serviço
└── ValueObjects/                      # (Preparado para futuros value objects)
```

### 2. **CSIFlex.Application** (Camada de Aplicação)
Contém os serviços de aplicação e DTOs para comunicação entre camadas.

**Estrutura:**
```
CSIFlex.Application/
├── DTOs/
│   ├── LoginDto.cs                    # DTO de requisição de login
│   └── AuthenticationResultDto.cs     # DTO de resposta de autenticação
├── Services/
│   └── AuthenticationService.cs       # Serviço de autenticação
└── Interfaces/                        # (Preparado para interfaces de aplicação)
```

### 3. **CSIFlex.Infrastructure** (Camada de Infraestrutura)
Implementa o acesso a dados, repositórios e serviços de infraestrutura.

**Estrutura:**
```
CSIFlex.Infrastructure/
├── Data/
│   └── DatabaseContext.cs             # Contexto de banco de dados
├── Repositories/
│   └── UserRepository.cs              # Implementação do repositório
└── Security/
    └── PasswordHasher.cs              # Utilitário de hash de senha
```

### 4. **CSIFlex.Web** (Camada de Apresentação)
Aplicação Blazor Server com interface de usuário.

**Estrutura:**
```
CSIFlex.Web/
├── Components/
│   ├── Pages/
│   │   └── Login.razor                # Página de login
│   ├── Layout/                        # Layouts padrão Blazor
│   ├── App.razor                      # Componente raiz
│   └── _Imports.razor                 # Imports globais
├── Services/
│   └── CustomAuthenticationStateProvider.cs  # Provedor de autenticação
├── wwwroot/                           # Arquivos estáticos
├── Program.cs                         # Configuração da aplicação
└── appsettings.json                   # Configurações centralizadas
```

## Funcionalidades Implementadas

### ✅ 4.1.1 Login de Usuário

A funcionalidade de login está completamente implementada com:

- **Autenticação segura** com hash PBKDF2 e salt
- **Validação de credenciais** contra o banco de dados MySQL
- **Verificação de permissões** (apenas administradores podem acessar)
- **Interface responsiva** com Bootstrap 5
- **Feedback visual** de erros e loading
- **Opção "Lembrar-me"** para persistência de sessão

## Configuração

### Pré-requisitos

- .NET 8 SDK
- MySQL Server 5.7 ou superior
- Banco de dados `csi_auth` com a tabela `users`

### Configuração do Banco de Dados

1. Edite o arquivo `CSIFlex.Web/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=csi_auth;Uid=root;Pwd=SUA_SENHA;SslMode=none;AllowPublicKeyRetrieval=true;CharSet=utf8mb4;"
  }
}
```

2. Certifique-se de que a tabela `users` existe no banco `csi_auth`:

```sql
CREATE TABLE IF NOT EXISTS `users` (
  `username_` VARCHAR(50) PRIMARY KEY,
  `password_` TEXT,
  `salt_` TEXT,
  `firstname_` VARCHAR(100),
  `Name_` VARCHAR(100),
  `displayname` VARCHAR(100),
  `email_` VARCHAR(100),
  `usertype` VARCHAR(20),
  `refId` VARCHAR(50),
  `title` VARCHAR(100),
  `dept` VARCHAR(100),
  `machines` TEXT,
  `phoneext` VARCHAR(20),
  `EditTimeline` BOOLEAN DEFAULT FALSE,
  `EditMasterPartData` BOOLEAN DEFAULT FALSE
);
```

## Execução

### Desenvolvimento

```bash
cd CSIFlex.Web
dotnet run
```

A aplicação estará disponível em: `https://localhost:5001` ou `http://localhost:5000`

### Produção

```bash
cd CSIFlex.Web
dotnet publish -c Release -o ./publish
```

## Princípios Aplicados

### DDD (Domain-Driven Design)
- Separação clara entre domínio, aplicação e infraestrutura
- Entidades ricas com comportamento encapsulado
- Repositórios para abstração de acesso a dados

### SOLID
- **S**ingle Responsibility: Cada classe tem uma única responsabilidade
- **O**pen/Closed: Extensível através de interfaces
- **L**iskov Substitution: Interfaces bem definidas
- **I**nterface Segregation: Interfaces específicas por contexto
- **D**ependency Inversion: Dependência de abstrações, não de implementações

### KISS (Keep It Simple, Stupid)
- Código claro e direto
- Evita complexidade desnecessária
- Fácil de entender e manter

### Clean Code
- Nomes descritivos e significativos
- Funções pequenas e focadas
- Comentários apenas onde necessário
- Código auto-explicativo

## Segurança

### Hash de Senhas
- Algoritmo: **PBKDF2** com SHA256
- Salt: 32 bytes (256 bits) único por usuário
- Iterações: 10.000
- Hash: 32 bytes (256 bits)
- Armazenamento: Base64

### Autenticação
- Sessão protegida com `ProtectedSessionStorage`
- Verificação de tipo de usuário (admin only)
- Comparação de hash em tempo constante (proteção contra timing attacks)

## Próximos Passos

As próximas funcionalidades a serem implementadas seguindo a análise do projeto original:

1. **Dashboard Principal** - Visualização de máquinas em tempo real
2. **Gerenciamento de Conectores** - CRUD de máquinas MTConnect/Focas
3. **Gerenciamento de Usuários** - CRUD de usuários
4. **Relatórios Automáticos** - Configuração e agendamento
5. **Gerenciamento de Licenças** - Validação e aplicação de licenças
6. **Configurações de Dashboard** - Personalização de visualização
7. **Unidades de Monitoramento** - Gerenciamento de sensores

## Estrutura de Banco de Dados

O projeto mantém compatibilidade com o banco de dados original do CSIFLEX. As principais tabelas utilizadas são:

- `csi_auth.users` - Usuários do sistema
- `csi_auth.tbl_csiconnector` - Conectores de máquinas
- `csi_auth.tbl_license` - Licenças
- `csi_database.tbl_devices` - Dispositivos de dashboard
- `monitoring.monitoringboards` - Unidades de monitoramento

## Contribuição

Este projeto segue as boas práticas de desenvolvimento. Ao contribuir:

1. Mantenha a separação de camadas
2. Siga os princípios SOLID e DDD
3. Escreva código limpo e auto-explicativo
4. Adicione comentários XML para APIs públicas
5. Teste suas alterações

## Licença

© 2026 CSIFLEX - Todos os direitos reservados
