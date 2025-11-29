@echo off
setlocal

set PID_FILE=mousejiggler.pid

if not exist "%PID_FILE%" (
    echo Mouse Jiggler non e' attivo ^(file PID non trovato^)
    timeout /t 3 >nul
    exit /b 1
)

set /p PID=<"%PID_FILE%"

echo Stopping Mouse Jiggler ^(PID: %PID%^)...

taskkill /PID %PID% /F >nul 2>&1

if %ERRORLEVEL% equ 0 (
    echo Mouse Jiggler fermato con successo!
    del "%PID_FILE%" >nul 2>&1
) else (
    echo Errore: impossibile fermare il processo ^(forse gia' terminato^)
    del "%PID_FILE%" >nul 2>&1
)

timeout /t 2 >nul
exit /b 0
