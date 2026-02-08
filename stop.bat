@echo off
:: =========================================================================
:: 🛑 WorkPlace Tasks — Stop Script
:: =========================================================================
:: Este script termina todos os processos da aplicação.
:: =========================================================================

echo ========================================
echo  WorkPlace Tasks - Stop Script
echo ========================================
echo.

echo [STOP] A terminar todos os servicos...
echo.

:: Matar processos dotnet.exe (Backend)
tasklist /FI "IMAGENAME eq dotnet.exe" 2>NUL | find /I /N "dotnet.exe">NUL
if "%ERRORLEVEL%"=="0" (
    echo [BACKEND] A terminar processos dotnet.exe...
    taskkill /F /IM dotnet.exe >NUL 2>&1
    echo [OK] Backend terminado.
) else (
    echo [INFO] Nenhum processo dotnet.exe encontrado.
)

echo.

:: Matar processos node.exe (Frontend)
tasklist /FI "IMAGENAME eq node.exe" 2>NUL | find /I /N "node.exe">NUL
if "%ERRORLEVEL%"=="0" (
    echo [FRONTEND] A terminar processos node.exe...
    taskkill /F /IM node.exe >NUL 2>&1
    echo [OK] Frontend terminado.
) else (
    echo [INFO] Nenhum processo node.exe encontrado.
)

echo.
echo ========================================
echo  ✅ Todos os servicos foram terminados
echo ========================================
echo.

pause
