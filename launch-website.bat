@echo off
echo.
echo ========================================
echo   MediCare Plus Medical Clinic Website
echo ========================================
echo.
echo Opening your medical clinic website...
echo.

REM Try to open with default browser
start "" "index.html"

echo Website should be opening in your default browser!
echo.
echo If you want to run with a local server instead:
echo 1. Run: powershell -ExecutionPolicy Bypass -File start-server.ps1
echo 2. Or visit: file:///%~dp0index.html
echo.
echo Press any key to exit...
pause >nul
