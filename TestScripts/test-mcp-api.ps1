Write-Host "Testing MCP API..." -ForegroundColor Green
Write-Host "1. Listing all tools..." -ForegroundColor Yellow
curl -X POST https://localhost:5001/mcp -H "Content-Type: application/json" -d "@request.json" -k
Write-Host "`n2. Creating a test case..." -ForegroundColor Yellow
curl -X POST https://localhost:5001/mcp -H "Content-Type: application/json" -d "@create-case.json" -k
Write-Host "`n3. Getting all cases..." -ForegroundColor Yellow
curl -X POST https://localhost:5001/mcp -H "Content-Type: application/json" -d "@get-all-cases.json" -k
Write-Host "`n4. Searching for test cases..." -ForegroundColor Yellow
curl -X POST https://localhost:5001/mcp -H "Content-Type: application/json" -d "@search-cases.json" -k
Write-Host "`n5. Updating the test case..." -ForegroundColor Yellow
curl -X POST https://localhost:5001/mcp -H "Content-Type: application/json" -d "@update-case.json" -k
Write-Host "`n6. Getting the specific case..." -ForegroundColor Yellow
curl -X POST https://localhost:5001/mcp -H "Content-Type: application/json" -d "@get-case.json" -k
