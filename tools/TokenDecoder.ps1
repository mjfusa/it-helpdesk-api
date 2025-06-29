# Token Decoder Script

# Function to decode JWT and display its claims
function Decode-JWT {
    param (
        [Parameter(Mandatory=$true)]
        [string]$Token
    )

    # Split the token into header, payload, and signature
    $tokenParts = $Token -split '\.'
    if ($tokenParts.Count -ne 3) {
        Write-Error "Invalid JWT format"
        return
    }

    # Function to decode base64url
    function ConvertFrom-Base64Url {
        param (
            [string]$Base64Url
        )
        
        # Add padding if needed
        $padding = 4 - ($Base64Url.Length % 4)
        if ($padding -ne 4) {
            $Base64Url += "=" * $padding
        }
        
        # Replace base64url chars with standard base64
        $base64 = $Base64Url.Replace('-', '+').Replace('_', '/')
        
        # Decode
        [System.Text.Encoding]::UTF8.GetString([Convert]::FromBase64String($base64))
    }

    # Decode header and payload
    $header = ConvertFrom-Base64Url -Base64Url $tokenParts[0]
    $payload = ConvertFrom-Base64Url -Base64Url $tokenParts[1]

    # Convert to JSON objects
    $headerJson = $header | ConvertFrom-Json
    $payloadJson = $payload | ConvertFrom-Json

    Write-Host "Token Header:" -ForegroundColor Cyan
    $headerJson | Format-List

    Write-Host "Token Payload:" -ForegroundColor Green
    $payloadJson | Format-List

    # Extract and display the audience claim specifically
    Write-Host "Audience (aud) claim:" -ForegroundColor Yellow
    $payloadJson.aud

    # Extract and display the issuer claim specifically
    Write-Host "Issuer (iss) claim:" -ForegroundColor Magenta
    $payloadJson.iss
}

# Instructions
Write-Host "To use this script, obtain an access token and then run:" -ForegroundColor White
Write-Host "Decode-JWT -Token 'your_jwt_token_here'" -ForegroundColor White
Write-Host ""
Write-Host "You can obtain a token by:"
Write-Host "1. Using Postman's 'Get New Access Token' feature with your Entra ID app"
Write-Host "2. Using Swagger UI's Authorize button"
Write-Host "3. Using Azure CLI: az account get-access-token --resource api://3b0fdb7e-3367-4307-a72b-e0c8ca4501f1"
Write-Host ""
Write-Host "Compare the token's 'aud' claim with the 'AzureAd:Audience' value in your appsettings.json"
