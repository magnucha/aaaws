# Managed system identity in Azure

**Lab: Create and Deploy a Web App using Azure Managed System Identity (MSI)**

**Objective:**

In this lab, you will:

1. Create a web app that lists all the Azure AD users.
2. Create a web app in Azure.
3. Grant the app permissions.
4. Enable MSI for the web app.
5. Run the web app locally and verify its functionality.
6. Deploy the web app to Azure.
7. Run the web app in Azure and verify its functionality.

**Lab Steps:**

**1. Create a Web App:**

Refer to the previous lab to create a web app that lists all Azure AD users. Update it to use `DefaultAzureCredential` like discussed.

**2. Create a Web App in Azure:**

Open a terminal window and log into Azure, and set the current subscription:

```pwsh
az login
az account set --subscription sandbox
```

Then, create a resource group:

```pwsh
$resourceGroupName = "{something-unique}"
az group create --name $resourceGroupName --location norwayeast 
```

Now, create a web app within this resource group:

```pwsh
$appName = <app-name>
az webapp create --resource-group $resourceGroupName --plan OurAppPlan --name $appName --runtime "dotnet:7" 
```

Replace `<app-name>` with a unique name for your web app. The name has to be globally unique, so don't make it too simple. Including your "u00" username should help.

To help with debugging, set it's environment to `Development`:

```pwsh
az webapp config appsettings set --name $appName --resource-group $resourceGroupName --settings ASPNETCORE_ENVIRONMENT=Development
```

Verify that it's running:

```pwsh
start "https://$appName.azurewebsites.net"
```

**3. Enable Managed System Identity for the Web App:**

```pwsh
az webapp identity assign --name $appName --resource-group $resourceGroupName
```

Make a note of the `principalId` value returned. 

Managed System Identity (MSI) is a feature of Azure Active Directory. It provides Azure services with an automatically managed identity in Azure AD. This can be used to authenticate to any service that supports Azure AD authentication, including Key Vault, without any credential in your code.

**4. Run the Web App Locally and Verify That It Works:**

You can run the web app locally using the .NET Core CLI:

```pwsh
dotnet run
```

Open a browser and navigate to `https://localhost:{PORT}` to ensure it works as expected.

**5. Deploy the Web App to Azure:**

Using Azure CLI:

Publish your application:

```pwsh
dotnet publish --configuration Release --output ./publish
```

Then, deploy the published output to Azure:

```pwsh
compress-Archive .\publish\* publish.zip
 az webapp deploy --resource-group $resourceGroupName --name $appName --src-path publish.zip --type zip
```

If you run it now, it should crash, since the system identity does not have any permissions in the Graph. 

But don't run it, since that will cause the MSI to get a token which does not work, and cache that for an hour. 

Let's add those permissions.

First, we need some values for our API call: 

```pwsh
$graphAppId = "00000003-0000-0000-c000-000000000000"    
$servicePrincipal = az ad sp show --id $GraphAppId | ConvertFrom-Json  
$appRole = $servicePrincipal.appRoles | Where-Object { $_.value -eq "user.read.all*" }

$principalId = az webapp identity show --name $appName --resource-group $resourceGroupName --query principalId -o tsv

$body = @{
    principalId = $principalId
    resourceId = $servicePrincipal.id
    appRoleId = $appRole.id
} | ConvertTo-Json

$body | Out-File -FilePath body.json -Encoding utf8
```

Use the Graph API and update the role assignments for our MSI

```pwsh
az rest --method POST --uri "https://graph.microsoft.com/v1.0/servicePrincipals/$principalId/appRoleAssignments" --headers 'Content-Type=application/json' --body `@body.json
```

**6. Run the App in Azure and Verify That It Works:**

Run the web app again and verify that it functions as expected.

Now we have an app running in the cloud, with it's own identity, no secrets floating about and with a specific permission that it can use.
