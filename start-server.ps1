# Simple PowerShell Web Server for MediCare Plus Website
# Run this script to start a local web server

$port = 8080
$url = "http://localhost:$port"

Write-Host "🏥 Starting MediCare Plus Medical Clinic Website..." -ForegroundColor Cyan
Write-Host "📡 Server starting on: $url" -ForegroundColor Green
Write-Host "📁 Serving files from: $(Get-Location)" -ForegroundColor Yellow
Write-Host "🌐 Press Ctrl+C to stop the server" -ForegroundColor Red
Write-Host ""

# Start browser
Start-Process $url

# Simple HTTP server using .NET HttpListener
Add-Type -AssemblyName System.Net.Http

$listener = New-Object System.Net.HttpListener
$listener.Prefixes.Add("$url/")

try {
    $listener.Start()
    Write-Host "✅ Server is running! Website available at: $url" -ForegroundColor Green
    Write-Host ""
    
    while ($listener.IsListening) {
        $context = $listener.GetContext()
        $request = $context.Request
        $response = $context.Response
        
        # Get requested file path
        $requestedFile = $request.Url.LocalPath.TrimStart('/')
        if ($requestedFile -eq '') { $requestedFile = 'index.html' }
        
        $filePath = Join-Path (Get-Location) $requestedFile
        
        if (Test-Path $filePath) {
            $content = Get-Content $filePath -Raw -Encoding UTF8
            
            # Set appropriate content type
            $contentType = switch -Wildcard ($requestedFile) {
                '*.html' { 'text/html; charset=utf-8' }
                '*.css'  { 'text/css; charset=utf-8' }
                '*.js'   { 'application/javascript; charset=utf-8' }
                '*.svg'  { 'image/svg+xml' }
                '*.json' { 'application/json' }
                default  { 'text/plain' }
            }
            
            $response.ContentType = $contentType
            $response.StatusCode = 200
            
            $buffer = [System.Text.Encoding]::UTF8.GetBytes($content)
            $response.ContentLength64 = $buffer.Length
            $response.OutputStream.Write($buffer, 0, $buffer.Length)
            
            Write-Host "📄 Served: $requestedFile" -ForegroundColor Gray
        } else {
            $response.StatusCode = 404
            $notFound = "File not found: $requestedFile"
            $buffer = [System.Text.Encoding]::UTF8.GetBytes($notFound)
            $response.ContentLength64 = $buffer.Length
            $response.OutputStream.Write($buffer, 0, $buffer.Length)
            
            Write-Host "❌ Not found: $requestedFile" -ForegroundColor Red
        }
        
        $response.Close()
    }
} catch {
    Write-Host "❌ Error: $($_.Exception.Message)" -ForegroundColor Red
} finally {
    if ($listener.IsListening) {
        $listener.Stop()
    }
    Write-Host "🛑 Server stopped." -ForegroundColor Yellow
}
