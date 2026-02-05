#!/bin/bash
# Script de teste de inicialização do CSIFLEX Server

echo "=========================================="
echo "  Teste de Inicialização - CSIFLEX Server"
echo "=========================================="
echo ""

cd "$(dirname "$0")/CSIFlex.Web"

echo "Compilando o projeto..."
dotnet build --configuration Release --verbosity quiet

if [ $? -ne 0 ]; then
    echo "❌ Erro na compilação!"
    exit 1
fi

echo "✅ Compilação bem-sucedida"
echo ""

echo "Iniciando o servidor (será encerrado após 10 segundos)..."
echo ""

# Iniciar o servidor em background
timeout 10s dotnet run --configuration Release --no-build 2>&1 &
PID=$!

# Aguardar 10 segundos
sleep 10

# Verificar se o processo ainda está rodando
if ps -p $PID > /dev/null 2>&1; then
    echo ""
    echo "✅ Servidor iniciou com sucesso!"
    echo ""
    echo "Encerrando servidor de teste..."
    kill $PID 2>/dev/null
    wait $PID 2>/dev/null
else
    echo ""
    echo "❌ Servidor não iniciou corretamente ou encerrou com erro"
    exit 1
fi

echo ""
echo "=========================================="
echo "  Teste Concluído"
echo "=========================================="
