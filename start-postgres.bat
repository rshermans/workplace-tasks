@echo off
:: Script para iniciar PostgreSQL com privilégios de administrador

echo Iniciando PostgreSQL...
net start postgresql-x64-18

if %errorlevel% equ 0 (
    echo PostgreSQL iniciado com sucesso!
) else (
    echo Erro ao iniciar PostgreSQL.
    echo Verifique se o servico existe ou se ja esta a correr.
    
    :: Mostrar status do serviço
    sc query postgresql-x64-18
)

pause
