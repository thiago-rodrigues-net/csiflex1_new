#!/bin/bash
# Script de inicialização do CSIFLEX Server para Linux/Mac
# Este script inicia o servidor automaticamente

echo "========================================"
echo "   CSIFLEX Server - Iniciando..."
echo "========================================"
echo ""

# Navegar para o diretório do script
cd "$(dirname "$0")"

# Verificar se o .NET está instalado
if ! command -v dotnet &> /dev/null; then
    echo "ERRO: .NET 8 SDK não encontrado!"
    echo "Por favor, instale o .NET 8 SDK de: https://dotnet.microsoft.com/download/dotnet/8.0"
    exit 1
fi

echo ".NET SDK encontrado:"
dotnet --version
echo ""

# Navegar para o diretório do projeto Web
cd CSIFlex.Web

# Restaurar pacotes NuGet (se necessário)
echo "Restaurando pacotes NuGet..."
dotnet restore --verbosity quiet

# Compilar o projeto
echo "Compilando o projeto..."
dotnet build --configuration Release --verbosity quiet --no-restore

if [ $? -ne 0 ]; then
    echo "ERRO: Falha na compilação do projeto!"
    exit 1
fi

echo ""
echo "========================================"
echo "   Servidor iniciado com sucesso!"
echo "========================================"
echo ""
echo "Acesse a aplicação em:"
echo "  HTTP:  http://localhost:5000"
echo "  HTTPS: https://localhost:5001"
echo ""
echo "Pressione Ctrl+C para parar o servidor"
echo "========================================"
echo ""

# Executar a aplicação
dotnet run --configuration Release --no-build
