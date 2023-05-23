# Creating a Deamon application

Let's create a .NET Core console application (a daemon app) to access the Microsoft Graph API.

**Step 1: Register an App in Azure AD**

1. Navigate to the Azure portal, then to Azure Active Directory > App registrations.
2. Click "New registration", enter a name for the app, and click "Register".
3. Under "Manage", go to "Certificates & secrets", click "New client secret", and note down the value, which we'll need later.

If you have PowerShell, you can run this script *instead* to create the app. Make sure to edit the app name to something that will not collide with the other workshop attendees.  

```pwsh
# Create an Azure AD App Registration and read App ID into a variable
$appReg = az ad app create --display-name "GraphDaemonApp" | ConvertFrom-Json
$appId = $appReg.appId

# Print out the App ID
Write-Output "App ID: $appId"

# Create a Client Secret
$clientSecret = az ad app credential reset --id $appId | ConvertFrom-Json

# Print out the client secret
Write-Output "Client Secret: $($clientSecret.password)"

```

**Add Graph permissions to the app:**

1. Under "Manage", click on "API permissions". 
2. Click "+ Add Permissions"
3. Select "Microsoft Graph" and select "Application permissions"
4. Add the Microsoft Graph permissions your daemon needs. For this lab, we'll select "User.Read.All".
5. Click "Add" to add the permission.

**Step 2: Grant Admin Consent**

1. Still under "API permissions", click "Grant admin consent for {Your Directory}".
2. Confirm by clicking "Yes".

We need to do this both because it's a high powered permission, and because the deamon app cannot consent on it's own, since it's non-interactive.

**Step 3: Create a .NET Core Console App**

Create a new Console Application project:

```bash
dotnet new console -n GraphDaemonApp
cd GraphDaemonApp
```

Install the necessary packages:

```bash
dotnet add package Azure.Identity
dotnet add package Microsoft.Extensions.Configuration.FileExtensions
dotnet add package Microsoft.Extensions.Configuration.Json
dotnet add package Microsoft.Graph
```

**Step 4: Configure and Code the App**

Add an `appsettings.json` file, add the following and replace placeholders with the actual values from your app registration:

```json
{
  "ClientId": "{Your-App-Client-Id}",
  "TenantId": "a1c5bee8-9e19-44cb-9f07-b94b279890ab",
  "ClientSecret": "{Your-Client-Secret}",
  "Authority": "https://login.microsoftonline.com/aaaaws.org",
  "Scope": "https://graph.microsoft.com/.default"
}
```

In `Program.cs`, replace the contents with the following code:

```csharp
using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;

var config = LoadAppSettings();

var credential = new ClientSecretCredential(config["TenantId"], config["ClientId"], config["ClientSecret"]);

var graphClient = new GraphServiceClient(credential);

var users = await graphClient.Users.GetAsync();
if (users?.Value != null)
{
    foreach (var user in users.Value)
    {
        Console.WriteLine($"{user.DisplayName} ({user.UserPrincipalName})");
    }
}

static IConfigurationRoot LoadAppSettings()
{
    var appConfig = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", false, true)
        .Build();

    // Check for required settings
    if (string.IsNullOrEmpty(appConfig["ClientId"]) ||
        string.IsNullOrEmpty(appConfig["TenantId"]) ||
        string.IsNullOrEmpty(appConfig["Authority"]) ||
        string.IsNullOrEmpty(appConfig["ClientSecret"]) ||
        string.IsNullOrEmpty(appConfig["Scope"]))
    {
        throw new Exception("Missing or invalid appSettings.json");
    }

    return appConfig;
}
```

**Step 5: Run the Daemon App**

Run the console

 app:

```bash
dotnet run
```

This will print out a list of all users in the tenant, assuming you've provided the "User.Read.All" permission and granted admin consent.

Remember, this is a simple example. In a production scenario, be sure to handle exceptions and security appropriately.
