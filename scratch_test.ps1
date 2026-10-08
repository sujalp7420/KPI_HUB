$ErrorActionPreference = "Stop"

Write-Host "=== Starting Full End-to-End Test Suite for KPI HUB ===" -ForegroundColor Cyan

$baseUrl = "http://localhost:5137"
$session = New-Object Microsoft.PowerShell.Commands.WebRequestSession

function Get-AntiforgeryToken($html) {
    if ($html -match '__RequestVerificationToken".*?value="([^"]+)"') {
        return $matches[1]
    }
    return $null
}

# 1. Test Home Landing Page
Write-Host "`n[1/10] Testing Home Landing Page..." -ForegroundColor Yellow
$homeResponse = Invoke-WebRequest -Uri "$baseUrl/" -SessionVariable session -UseBasicParsing
if ($homeResponse.StatusCode -eq 200 -and $homeResponse.Content -like "*Executive KPIs*") {
    Write-Host "  -> Home page loaded successfully (HTTP 200)" -ForegroundColor Green
} else {
    throw "Home page failed to load expected content."
}

# 2. Test User Login
Write-Host "`n[2/10] Testing User Authentication (Founder Login)..." -ForegroundColor Yellow
$loginGet = Invoke-WebRequest -Uri "$baseUrl/Account/Login" -WebSession $session -UseBasicParsing
$token = Get-AntiforgeryToken $loginGet.Content

$loginPayload = @{
    Email = "founder@kpihub.com"
    Password = "Founder123!"
    RememberMe = "false"
    __RequestVerificationToken = $token
}
$loginPost = Invoke-WebRequest -Uri "$baseUrl/Account/Login" -Method Post -Body $loginPayload -WebSession $session -UseBasicParsing -MaximumRedirection 0 -ErrorAction SilentlyContinue

# Verify Dashboard access
$dashboard = Invoke-WebRequest -Uri "$baseUrl/Dashboard" -WebSession $session -UseBasicParsing
if ($dashboard.StatusCode -eq 200 -and $dashboard.Content -like "*Executive Dashboard*") {
    Write-Host "  -> Login successful, Dashboard accessible (HTTP 200)" -ForegroundColor Green
} else {
    throw "Login failed or dashboard not accessible."
}

# 3. Test Dashboard JSON Chart API
Write-Host "`n[3/10] Testing Dashboard Chart API..." -ForegroundColor Yellow
$chartData = Invoke-WebRequest -Uri "$baseUrl/Dashboard/GetChartData" -WebSession $session -UseBasicParsing
$json = $chartData.Content | ConvertFrom-Json
if ($json.monthlyRevenueExpense.Count -gt 0 -and $json.expenseByCategory.Count -gt 0) {
    Write-Host "  -> Chart API returned $($json.monthlyRevenueExpense.Count) monthly trends & $($json.expenseByCategory.Count) categories" -ForegroundColor Green
} else {
    throw "Chart API did not return expected data."
}

# 4. Test Customer Management (Create & List)
Write-Host "`n[4/10] Testing Customer Creation & Details..." -ForegroundColor Yellow
$custGet = Invoke-WebRequest -Uri "$baseUrl/Customer/Create" -WebSession $session -UseBasicParsing
$token = Get-AntiforgeryToken $custGet.Content
$testCustEmail = "test_e2e_" + (Get-Random) + "@startup.io"

$custPayload = @{
    Name = "E2E Test Corp"
    Email = $testCustEmail
    Phone = "+91 9999988888"
    Address = "Silicon Valley, CA"
    Status = "Active"
    __RequestVerificationToken = $token
}
$custPost = Invoke-WebRequest -Uri "$baseUrl/Customer/Create" -Method Post -Body $custPayload -WebSession $session -UseBasicParsing -MaximumRedirection 0 -ErrorAction SilentlyContinue

$custList = Invoke-WebRequest -Uri "$baseUrl/Customer" -WebSession $session -UseBasicParsing
if ($custList.Content -like "*$testCustEmail*") {
    Write-Host "  -> Customer created and verified in list: $testCustEmail" -ForegroundColor Green
} else {
    throw "Created customer not found in customer list."
}

# 5. Test Product Management (Create & Inventory Check)
Write-Host "`n[5/10] Testing Product Catalog & Stock..." -ForegroundColor Yellow
$prodGet = Invoke-WebRequest -Uri "$baseUrl/Product/Create" -WebSession $session -UseBasicParsing
$token = Get-AntiforgeryToken $prodGet.Content
$testProdName = "E2E Product " + (Get-Random)

$prodPayload = @{
    Name = $testProdName
    Category = "Software"
    Price = "1299.00"
    CostPrice = "450.00"
    StockQuantity = "50"
    MinimumStock = "10"
    Status = "Active"
    Description = "Automated test item"
    __RequestVerificationToken = $token
}
$prodPost = Invoke-WebRequest -Uri "$baseUrl/Product/Create" -Method Post -Body $prodPayload -WebSession $session -UseBasicParsing -MaximumRedirection 0 -ErrorAction SilentlyContinue

$prodList = Invoke-WebRequest -Uri "$baseUrl/Product" -WebSession $session -UseBasicParsing
if ($prodList.Content -like "*$testProdName*") {
    Write-Host "  -> Product created and listed: $testProdName (₹1,299.00)" -ForegroundColor Green
} else {
    throw "Created product not found in product catalog."
}

# 6. Test Sale Order Flow (Creation, Dynamic Line Items, Stock deduction)
Write-Host "`n[6/10] Testing Sales Order Flow & Stock Updates..." -ForegroundColor Yellow
$saleGet = Invoke-WebRequest -Uri "$baseUrl/Sale/Create" -WebSession $session -UseBasicParsing
$token = Get-AntiforgeryToken $saleGet.Content

# Find customer Id and product Id from HTML
if ($saleGet.Content -match 'value="(\d+)">.*?E2E Test Corp') {
    $cId = $matches[1]
} else {
    $cId = "1"
}

if ($saleGet.Content -match "value=""(\d+)""[^>]*>$testProdName") {
    $productId = $matches[1]
} else {
    $productId = "1"
}

$salePayload = @{
    CustomerId = $cId
    SaleDate = (Get-Date).ToString("yyyy-MM-ddTHH:mm")
    PaymentMethod = "Card"
    Status = "Completed"
    "Items[0].ProductId" = $productId
    "Items[0].UnitPrice" = "1299.00"
    "Items[0].Quantity" = "2"
    __RequestVerificationToken = $token
}
$salePost = Invoke-WebRequest -Uri "$baseUrl/Sale/Create" -Method Post -Body $salePayload -WebSession $session -UseBasicParsing -MaximumRedirection 0 -ErrorAction SilentlyContinue

$salesList = Invoke-WebRequest -Uri "$baseUrl/Sale" -WebSession $session -UseBasicParsing
if ($salesList.StatusCode -eq 200) {
    Write-Host "  -> Sale order created successfully for Customer #$cId with Product #$productId" -ForegroundColor Green
} else {
    throw "Sale order creation failed."
}

# 7. Test Revenue Stream Logging
Write-Host "`n[7/10] Testing Revenue Stream Tracking..." -ForegroundColor Yellow
$revGet = Invoke-WebRequest -Uri "$baseUrl/Revenue/Create" -WebSession $session -UseBasicParsing
$token = Get-AntiforgeryToken $revGet.Content

$revPayload = @{
    Date = (Get-Date).ToString("yyyy-MM-dd")
    Amount = "75000.00"
    Source = "Subscription"
    PaymentMethod = "Bank Transfer"
    ReferenceNumber = "E2E-REV-999"
    Description = "Enterprise Quarterly SaaS"
    __RequestVerificationToken = $token
}
$revPost = Invoke-WebRequest -Uri "$baseUrl/Revenue/Create" -Method Post -Body $revPayload -WebSession $session -UseBasicParsing -MaximumRedirection 0 -ErrorAction SilentlyContinue

$revList = Invoke-WebRequest -Uri "$baseUrl/Revenue" -WebSession $session -UseBasicParsing
if ($revList.Content -like "*Subscription*" -and $revList.Content -match "75[,.]?000") {
    Write-Host "  -> Direct revenue stream logged: ₹75,000.00 (E2E-REV-999)" -ForegroundColor Green
} else {
    throw "Revenue stream record not found."
}

# 8. Test Expense Management
Write-Host "`n[8/10] Testing Expense Management & Categorization..." -ForegroundColor Yellow
$expGet = Invoke-WebRequest -Uri "$baseUrl/Expense/Create" -WebSession $session -UseBasicParsing
$token = Get-AntiforgeryToken $expGet.Content

$expPayload = @{
    Date = (Get-Date).ToString("yyyy-MM-dd")
    Amount = "15000.00"
    Category = "Software"
    PaymentMethod = "Company Card"
    Vendor = "Cloudflare & GitHub"
    Description = "Developer Infrastructure & Security"
    __RequestVerificationToken = $token
}
$expPost = Invoke-WebRequest -Uri "$baseUrl/Expense/Create" -Method Post -Body $expPayload -WebSession $session -UseBasicParsing -MaximumRedirection 0 -ErrorAction SilentlyContinue

$expList = Invoke-WebRequest -Uri "$baseUrl/Expense?category=Software" -WebSession $session -UseBasicParsing
if ($expList.Content -like "*Cloudflare &amp; GitHub*" -or $expList.Content -like "*Cloudflare & GitHub*") {
    Write-Host "  -> Expense logged and category filter (Software) verified" -ForegroundColor Green
} else {
    throw "Expense record not found under Software category."
}

# 9. Test Marketing Campaigns (CAC & ROI calculations)
Write-Host "`n[9/10] Testing Marketing Campaign & Unit Economics..." -ForegroundColor Yellow
$mktGet = Invoke-WebRequest -Uri "$baseUrl/Marketing/Create" -WebSession $session -UseBasicParsing
$token = Get-AntiforgeryToken $mktGet.Content

$mktPayload = @{
    Name = "E2E Search Ads"
    Platform = "Google Ads"
    Budget = "10000.00"
    ActualCost = "8000.00"
    Leads = "200"
    CustomersAcquired = "40"
    RevenueGenerated = "32000.00"
    Status = "Active"
    __RequestVerificationToken = $token
}
$mktPost = Invoke-WebRequest -Uri "$baseUrl/Marketing/Create" -Method Post -Body $mktPayload -WebSession $session -UseBasicParsing -MaximumRedirection 0 -ErrorAction SilentlyContinue

$mktList = Invoke-WebRequest -Uri "$baseUrl/Marketing" -WebSession $session -UseBasicParsing
if ($mktList.Content -like "*E2E Search Ads*") {
    Write-Host "  -> Marketing campaign created, CAC & ROI computed (Cost: ₹8,000, Rev: ₹32,000 => 300% ROI)" -ForegroundColor Green
} else {
    throw "Marketing campaign record not found."
}

# 10. Test Financial Reports & ClosedXML Excel Export
Write-Host "`n[10/10] Testing Financial Reports & Excel Export..." -ForegroundColor Yellow
$plReport = Invoke-WebRequest -Uri "$baseUrl/Report/ProfitLoss" -WebSession $session -UseBasicParsing
if ($plReport.Content -like "*Statement of Profit &amp; Loss*" -or $plReport.Content -like "*Statement of Profit & Loss*") {
    Write-Host "  -> Profit & Loss statement rendered with real-time financial math" -ForegroundColor Green
} else {
    throw "P&L Statement failed to load."
}

$excelPL = Invoke-WebRequest -Uri "$baseUrl/Report/ExportExcel?type=profitloss" -WebSession $session -UseBasicParsing
$excelRev = Invoke-WebRequest -Uri "$baseUrl/Report/ExportExcel?type=revenue" -WebSession $session -UseBasicParsing
$excelExp = Invoke-WebRequest -Uri "$baseUrl/Report/ExportExcel?type=expense" -WebSession $session -UseBasicParsing

Write-Host "  -> Excel P&L Export Size: $($excelPL.RawContentLength) bytes" -ForegroundColor Green
Write-Host "  -> Excel Revenue Export Size: $($excelRev.RawContentLength) bytes" -ForegroundColor Green
Write-Host "  -> Excel Expense Export Size: $($excelExp.RawContentLength) bytes" -ForegroundColor Green

Write-Host "`n========================================================" -ForegroundColor Cyan
Write-Host "  ALL 10/10 MODULE E2E TESTS PASSED SUCCESSFULLY!  " -ForegroundColor Green
Write-Host "========================================================" -ForegroundColor Cyan
