param(
	[Parameter(Mandatory=$true)]
	[string] $TenantId,

	[Parameter(Mandatory=$true)]
	[string] $ClientId,

	[Parameter(Mandatory=$true)]
	[string] $ClientSecret
)

$headers = @{
	'Content-Type'	= 'application/x-www-form-urlencoded'
}
$body = @{
	'grant_type'	= 'client_credentials';
	'client_id'		= $ClientId;
	'client_secret'	= $ClientSecret;
	'resource'		= 'https://manage.devcenter.microsoft.com'
}
$uri = "https://login.microsoftonline.com/$TenantId/oauth2/token"

$response = Invoke-WebRequest -Method Post -Uri $uri -Headers $headers -Body $body

$token = ($response.Content | ConvertFrom-Json).access_token

Write-Host "##vso[task.setvariable variable=ADAccessToken;issecret=true]$token"
