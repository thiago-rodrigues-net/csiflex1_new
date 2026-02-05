# Guia Rápido de Inicialização - CSIFLEX Server

## 🚀 Início Rápido

### Windows

1. **Execute o script de inicialização:**
   ```cmd
   start-server.bat
   ```

2. **Acesse a aplicação:**
   - HTTP: http://localhost:5000
   - HTTPS: https://localhost:5001

3. **Faça login:**
   - Usuário: `admin`
   - Senha: `admin123`

### Linux / macOS

1. **Execute o script de inicialização:**
   ```bash
   ./start-server.sh
   ```

2. **Acesse a aplicação:**
   - HTTP: http://localhost:5000
   - HTTPS: https://localhost:5001

3. **Faça login:**
   - Usuário: `admin`
   - Senha: `admin123`

---

## ⚙️ Configuração Inicial

### 1. Configurar Banco de Dados

Antes de iniciar o servidor, configure a connection string no arquivo `CSIFlex.Web/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=csi_auth;Uid=root;Pwd=SUA_SENHA;SslMode=none;AllowPublicKeyRetrieval=true;CharSet=utf8mb4;"
  }
}
```

### 2. Criar Banco de Dados

Execute o script SQL fornecido:

```bash
mysql -u root -p < database_setup.sql
```

### 3. Criar Usuário Admin

Após criar o banco, você precisa inserir um usuário admin. Use o script fornecido ou insira manualmente.

---

## 🔧 Configuração do Kestrel

O servidor está configurado para escutar nas seguintes portas:

- **Porta 5000:** HTTP
- **Porta 5001:** HTTPS (com certificado de desenvolvimento)

Para alterar as portas, edite o arquivo `CSIFlex.Web/Program.cs`:

```csharp
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(5000); // HTTP
    serverOptions.ListenAnyIP(5001, listenOptions =>
    {
        listenOptions.UseHttps(); // HTTPS
    });
});
```

---

## 🐧 Executar como Serviço Linux (systemd)

### Instalação

1. **Publique a aplicação:**
   ```bash
   cd CSIFlex.Web
   dotnet publish -c Release -o /opt/csiflex
   ```

2. **Copie o arquivo de serviço:**
   ```bash
   sudo cp csiflex-server.service /etc/systemd/system/
   ```

3. **Edite o arquivo de serviço (se necessário):**
   ```bash
   sudo nano /etc/systemd/system/csiflex-server.service
   ```

4. **Recarregue o systemd:**
   ```bash
   sudo systemctl daemon-reload
   ```

5. **Habilite o serviço:**
   ```bash
   sudo systemctl enable csiflex-server
   ```

6. **Inicie o serviço:**
   ```bash
   sudo systemctl start csiflex-server
   ```

7. **Verifique o status:**
   ```bash
   sudo systemctl status csiflex-server
   ```

### Comandos Úteis

```bash
# Iniciar o serviço
sudo systemctl start csiflex-server

# Parar o serviço
sudo systemctl stop csiflex-server

# Reiniciar o serviço
sudo systemctl restart csiflex-server

# Ver logs
sudo journalctl -u csiflex-server -f

# Desabilitar inicialização automática
sudo systemctl disable csiflex-server
```

---

## 🪟 Executar como Serviço Windows

### Usando NSSM (Non-Sucking Service Manager)

1. **Baixe o NSSM:**
   - https://nssm.cc/download

2. **Publique a aplicação:**
   ```cmd
   cd CSIFlex.Web
   dotnet publish -c Release -o C:\CSIFLEX\Server
   ```

3. **Instale o serviço:**
   ```cmd
   nssm install CSIFlexServer "C:\Program Files\dotnet\dotnet.exe" "C:\CSIFLEX\Server\CSIFlex.Web.dll"
   ```

4. **Configure o serviço:**
   ```cmd
   nssm set CSIFlexServer AppDirectory C:\CSIFLEX\Server
   nssm set CSIFlexServer DisplayName "CSIFLEX Server"
   nssm set CSIFlexServer Description "Sistema de Monitoramento Industrial CSIFLEX"
   nssm set CSIFlexServer Start SERVICE_AUTO_START
   ```

5. **Inicie o serviço:**
   ```cmd
   nssm start CSIFlexServer
   ```

### Usando sc.exe (Nativo do Windows)

```cmd
sc create CSIFlexServer binPath="C:\CSIFLEX\Server\CSIFlex.Web.exe" start=auto
sc description CSIFlexServer "Sistema de Monitoramento Industrial CSIFLEX"
sc start CSIFlexServer
```

---

## 🔒 Certificado SSL para Produção

### Desenvolvimento

O certificado de desenvolvimento do .NET é usado automaticamente:

```bash
dotnet dev-certs https --trust
```

### Produção

Para produção, você deve usar um certificado SSL válido:

1. **Obtenha um certificado SSL** (Let's Encrypt, Certbot, etc.)

2. **Configure no Program.cs:**
   ```csharp
   builder.WebHost.ConfigureKestrel(serverOptions =>
   {
       serverOptions.ListenAnyIP(5001, listenOptions =>
       {
           listenOptions.UseHttps("/path/to/certificate.pfx", "password");
       });
   });
   ```

3. **Ou configure via appsettings.json:**
   ```json
   {
     "Kestrel": {
       "Endpoints": {
         "Https": {
           "Url": "https://*:5001",
           "Certificate": {
             "Path": "/path/to/certificate.pfx",
             "Password": "your-password"
           }
         }
       }
     }
   }
   ```

---

## 🌐 Acesso Remoto

Para permitir acesso de outras máquinas na rede:

1. **Configure o firewall:**
   ```bash
   # Linux (UFW)
   sudo ufw allow 5000/tcp
   sudo ufw allow 5001/tcp

   # Windows
   netsh advfirewall firewall add rule name="CSIFLEX HTTP" dir=in action=allow protocol=TCP localport=5000
   netsh advfirewall firewall add rule name="CSIFLEX HTTPS" dir=in action=allow protocol=TCP localport=5001
   ```

2. **Acesse via IP:**
   - http://SEU_IP:5000
   - https://SEU_IP:5001

---

## 📊 Monitoramento

### Logs da Aplicação

Os logs são gravados em:
- **Console:** Durante execução manual
- **Systemd Journal:** Quando executado como serviço Linux
- **Event Viewer:** Quando executado como serviço Windows

### Verificar Saúde do Servidor

```bash
# Verificar se o servidor está respondendo
curl http://localhost:5000

# Verificar processos
ps aux | grep dotnet

# Verificar portas abertas
netstat -tulpn | grep :5000
```

---

## 🛠️ Solução de Problemas

### Porta já em uso

```bash
# Linux
sudo lsof -i :5000
sudo kill -9 <PID>

# Windows
netstat -ano | findstr :5000
taskkill /PID <PID> /F
```

### Erro de permissão

```bash
# Linux - Dar permissão ao usuário
sudo chown -R www-data:www-data /opt/csiflex
sudo chmod -R 755 /opt/csiflex
```

### Certificado SSL inválido

```bash
# Recriar certificado de desenvolvimento
dotnet dev-certs https --clean
dotnet dev-certs https --trust
```

---

## 📞 Suporte

Para mais informações, consulte:
- **README.md** - Documentação completa
- **INSTALL.md** - Guia de instalação detalhado
- **PROJETO_RESUMO.md** - Resumo do projeto

---

**© 2026 CSIFLEX - Sistema de Monitoramento Industrial**
