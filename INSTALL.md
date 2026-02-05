# Guia de Instalação - CSIFLEX Server .NET 8

## Pré-requisitos

Antes de iniciar a instalação, certifique-se de ter:

- **.NET 8 SDK** instalado ([Download aqui](https://dotnet.microsoft.com/download/dotnet/8.0))
- **MySQL Server 5.7+** ou **MariaDB 10.3+**
- **Visual Studio 2022** (opcional, mas recomendado) ou **Visual Studio Code**

## Passo 1: Clonar o Repositório

```bash
git clone https://github.com/thiago-rodrigues-net/csiflex1_new.git
cd csiflex1_new
```

## Passo 2: Configurar o Banco de Dados

### 2.1 Criar o Banco de Dados

Execute o script SQL fornecido:

```bash
mysql -u root -p < database_setup.sql
```

Ou execute manualmente no MySQL Workbench/phpMyAdmin.

### 2.2 Criar Usuário Admin

Você precisa gerar um hash de senha para o usuário admin. Há duas opções:

#### Opção A: Usar o utilitário fornecido (recomendado)

```bash
# No diretório do projeto
dotnet run --project GenerateAdminHash.csproj
```

Isso irá gerar um hash para a senha padrão `admin123`.

#### Opção B: Executar SQL diretamente

Execute o seguinte SQL após gerar o hash:

```sql
INSERT INTO csi_auth.users (
  username_, password_, salt_, firstname_, Name_, 
  displayname, email_, usertype, EditTimeline, EditMasterPartData
) VALUES (
  'admin',
  'SEU_HASH_AQUI',
  'SEU_SALT_AQUI',
  'Administrator',
  'System',
  'Administrator',
  'admin@csiflex.com',
  'admin',
  TRUE,
  TRUE
);
```

## Passo 3: Configurar Connection String

Edite o arquivo `CSIFlex.Web/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=csi_auth;Uid=root;Pwd=SUA_SENHA_MYSQL;SslMode=none;AllowPublicKeyRetrieval=true;CharSet=utf8mb4;"
  }
}
```

**Importante:** Substitua `SUA_SENHA_MYSQL` pela senha real do seu MySQL.

## Passo 4: Restaurar Pacotes NuGet

```bash
cd csiflex1_new
dotnet restore
```

## Passo 5: Compilar o Projeto

```bash
dotnet build
```

Verifique se não há erros de compilação.

## Passo 6: Executar a Aplicação

### Modo Desenvolvimento

```bash
cd CSIFlex.Web
dotnet run
```

A aplicação estará disponível em:
- HTTPS: https://localhost:5001
- HTTP: http://localhost:5000

### Modo Produção

```bash
cd CSIFlex.Web
dotnet publish -c Release -o ./publish
cd publish
dotnet CSIFlex.Web.dll
```

## Passo 7: Primeiro Acesso

1. Abra o navegador e acesse: `https://localhost:5001/login`
2. Faça login com as credenciais:
   - **Usuário:** `admin`
   - **Senha:** `admin123` (ou a senha que você configurou)
3. **IMPORTANTE:** Altere a senha após o primeiro login!

## Configuração Adicional

### Configurar HTTPS (Produção)

Para produção, você deve configurar um certificado SSL válido:

```bash
dotnet dev-certs https --trust
```

### Configurar como Serviço Windows

Para executar como serviço do Windows:

1. Publique a aplicação:
```bash
dotnet publish -c Release -o C:\CSIFLEX\Server
```

2. Instale como serviço usando `sc.exe` ou NSSM:
```cmd
sc create CSIFlexServer binPath="C:\CSIFLEX\Server\CSIFlex.Web.exe"
sc start CSIFlexServer
```

### Configurar como Serviço Linux (systemd)

1. Crie o arquivo `/etc/systemd/system/csiflex.service`:

```ini
[Unit]
Description=CSIFLEX Server Application
After=network.target

[Service]
Type=notify
WorkingDirectory=/opt/csiflex
ExecStart=/usr/bin/dotnet /opt/csiflex/CSIFlex.Web.dll
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=csiflex
User=www-data
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

[Install]
WantedBy=multi-user.target
```

2. Habilite e inicie o serviço:

```bash
sudo systemctl enable csiflex
sudo systemctl start csiflex
sudo systemctl status csiflex
```

## Solução de Problemas

### Erro de Conexão com MySQL

**Problema:** `Unable to connect to any of the specified MySQL hosts`

**Solução:**
1. Verifique se o MySQL está rodando: `sudo systemctl status mysql`
2. Verifique a connection string no `appsettings.json`
3. Teste a conexão: `mysql -u root -p`

### Erro de Autenticação

**Problema:** `Authentication to host 'localhost' for user 'root' using method 'caching_sha2_password' failed`

**Solução:**
Adicione `AllowPublicKeyRetrieval=true` na connection string ou altere o método de autenticação:

```sql
ALTER USER 'root'@'localhost' IDENTIFIED WITH mysql_native_password BY 'sua_senha';
FLUSH PRIVILEGES;
```

### Porta já em uso

**Problema:** `Failed to bind to address https://127.0.0.1:5001`

**Solução:**
Altere a porta no arquivo `CSIFlex.Web/Properties/launchSettings.json`

### Erro de Certificado SSL

**Problema:** `The SSL connection could not be established`

**Solução:**
Para desenvolvimento, confie no certificado de desenvolvimento:

```bash
dotnet dev-certs https --trust
```

## Verificação da Instalação

Execute os seguintes comandos para verificar se tudo está funcionando:

```bash
# Verificar versão do .NET
dotnet --version

# Verificar se o projeto compila
dotnet build

# Verificar se os testes passam (quando implementados)
dotnet test

# Verificar conexão com MySQL
mysql -u root -p -e "USE csi_auth; SHOW TABLES;"
```

## Próximos Passos

Após a instalação bem-sucedida:

1. ✅ Altere a senha do usuário admin
2. ✅ Configure o backup automático do banco de dados
3. ✅ Configure o firewall para permitir acesso à porta 5001
4. ✅ Configure SSL com certificado válido para produção
5. ✅ Revise as configurações de segurança no `appsettings.json`

## Suporte

Para problemas ou dúvidas:
- Consulte o arquivo `README.md`
- Verifique os logs em `CSIFlex.Web/logs/`
- Entre em contato com o suporte técnico

---

**Versão:** 2.0.0  
**Data:** Fevereiro 2026  
**© CSIFLEX - Todos os direitos reservados**
