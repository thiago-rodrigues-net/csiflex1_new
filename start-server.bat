@echo off
REM Script de inicialização do CSIFLEX Server para Windows
REM Este script inicia o servidor automaticamente

echo ========================================
echo    CSIFLEX Server - Iniciando...
echo ========================================
echo.

REM Navegar para o diretório do projeto Web
cd /d "%~dp0CSIFlex.Web"

REM Verificar se o .NET está instalado
dotnet --version >nul 2>&1
if errorlevel 1 (
    echo ERRO: .NET 8 SDK nao encontrado!
    echo Por favor, instale o .NET 8 SDK de: https://dotnet.microsoft.com/download/dotnet/8.0
    pause
    exit /b 1
)

echo .NET SDK encontrado: 
dotnet --version
echo.

REM Restaurar pacotes NuGet (se necessário)
echo Restaurando pacotes NuGet...
dotnet restore --verbosity quiet

REM Compilar o projeto
echo Compilando o projeto...
dotnet build --configuration Release --verbosity quiet --no-restore

if errorlevel 1 (
    echo ERRO: Falha na compilacao do projeto!
    pause
    exit /b 1
)

echo.
echo ========================================
echo    Servidor iniciado com sucesso!
echo ========================================
echo.
echo Acesse a aplicacao em:
echo   HTTP:  http://localhost:5000
echo   HTTPS: https://localhost:5001
echo.
echo Pressione Ctrl+C para parar o servidor
echo ========================================
echo.

REM Executar a aplicação
dotnet run --configuration Release --no-build

pause
