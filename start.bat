@echo off
setlocal enabledelayedexpansion

:: =========================================================================
:: 🚀 WorkPlace Tasks — Automation & Cleanup Script
:: =========================================================================
:: Este script faz limpeza de processos anteriores e inicia a aplicação.
:: =========================================================================

echo ========================================
echo  WorkPlace Tasks - Startup Script
echo ========================================
echo.

:: =========================================================================
:: STEP 0: CLEANUP - Matar processos anteriores
:: =========================================================================
echo [CLEANUP] A terminar processos anteriores...

:: Matar processos dotnet.exe (Backend)
tasklist /FI "IMAGENAME eq dotnet.exe" 2>NUL | find /I /N "dotnet.exe">NUL
if "%ERRORLEVEL%"=="0" (
    echo [CLEANUP] A terminar processos dotnet.exe...
    taskkill /F /IM dotnet.exe >NUL 2>&1
    timeout /t 2 /nobreak >NUL
)

:: Matar processos node.exe (Frontend)
tasklist /FI "IMAGENAME eq node.exe" 2>NUL | find /I /N "node.exe">NUL
if "%ERRORLEVEL%"=="0" (
    echo [CLEANUP] A terminar processos node.exe...
    taskkill /F /IM node.exe >NUL 2>&1
    timeout /t 2 /nobreak >NUL
)

echo [CLEANUP] Limpeza concluida.
echo.

:: =========================================================================
:: STEP 1: VERIFICAR REQUISITOS
:: =========================================================================
echo [STEP 1] A verificar requisitos...

:: Verificar .NET SDK
dotnet --version >nul 2>&1
if %errorlevel% neq 0 (
    echo [ERROR] .NET SDK 8.0 nao encontrado. Por favor instale-o.
    pause
    exit /b 1
)

:: Verificar Node.js
node --version >nul 2>&1
if %errorlevel% neq 0 (
    echo [ERROR] Node.js nao encontrado. Por favor instale-o.
    pause
    exit /b 1
)

echo [OK] Requisitos verificados.
echo.

:: =========================================================================
:: STEP 2: INFRAESTRUTURA (Base de Dados)
:: =========================================================================
echo [STEP 2] A iniciar Base de Dados...

:: Tentar Docker primeiro
docker compose up -d db >nul 2>&1
if %errorlevel% equ 0 (
    echo [OK] PostgreSQL iniciado via Docker.
    goto db_done
)

echo [INFO] Docker nao disponivel. A tentar PostgreSQL local...

:: Verificar se existe serviço PostgreSQL local
sc query postgresql-x64-18 >nul 2>&1
if %errorlevel% neq 0 (
    echo [WARNING] Nenhum PostgreSQL encontrado.
    echo [INFO] A aplicacao usara SQLite como fallback automatico.
    goto db_done
)

:: Tentar iniciar o serviço PostgreSQL
net start postgresql-x64-18 >nul 2>&1
if %errorlevel% equ 0 (
    echo [OK] PostgreSQL local iniciado.
) else (
    echo [INFO] PostgreSQL local ja esta a correr ou requer privilegios admin.
)

:db_done
echo.

:: =========================================================================
:: STEP 3: PREPARAÇÃO DO BACKEND (Build + Migrações)
:: =========================================================================
echo [STEP 3] A preparar Backend...

cd backend\WorkplaceTasks.Api

:: Fazer clean build para evitar problemas de cache
echo [BUILD] A fazer clean build...
dotnet clean >NUL 2>&1
dotnet build --no-restore >NUL 2>&1

:: Aplicar migrações
echo [MIGRATIONS] A aplicar migracoes do Entity Framework...
dotnet ef database update
if %errorlevel% neq 0 (
    echo [WARNING] Erro ao aplicar migracoes. Verifique a ligacao a base de dados.
)

cd ..\..
echo.

:: =========================================================================
:: STEP 4: PREPARAÇÃO DO FRONTEND
:: =========================================================================
echo [STEP 4] A preparar Frontend...

cd frontend\workplace-tasks-web

:: Verificar e instalar dependências se necessário
if not exist "node_modules" (
    echo [NPM] A instalar dependencias...
    call npm install
    if %errorlevel% neq 0 (
        echo [ERROR] Falha ao instalar dependencias do frontend.
        cd ..\..
        pause
        exit /b 1
    )
)

cd ..\..
echo.

:: =========================================================================
:: STEP 5: ARRANQUE DOS SERVIÇOS
:: =========================================================================
echo [STEP 5] A iniciar servicos...
echo.

:: Iniciar Backend num novo terminal
echo [BACKEND] A iniciar API em nova janela...
start "Backend - Workplace API" cmd /k "cd /d %CD%\backend\WorkplaceTasks.Api && dotnet run"

:: Aguardar 3 segundos para o backend iniciar
echo [INFO] A aguardar inicializacao do backend...
timeout /t 3 /nobreak >NUL

:: Iniciar Frontend num novo terminal
echo [FRONTEND] A iniciar React App em nova janela...
start "Frontend - React App" cmd /k "cd /d %CD%\frontend\workplace-tasks-web && npm run dev"

echo.
echo ========================================
echo  ✅ Aplicacao iniciada com sucesso!
echo ========================================
echo.
echo 📍 URLs:
echo    Backend API:  https://localhost:5001/swagger
echo    Frontend App: http://localhost:5174
echo.
echo 💡 Duas janelas foram abertas:
echo    - Backend (dotnet run)
echo    - Frontend (npm run dev)
echo.
echo ⚠️  Para parar a aplicacao, feche ambas as janelas.
echo.
echo Pressione qualquer tecla para fechar este terminal...
pause >NUL
