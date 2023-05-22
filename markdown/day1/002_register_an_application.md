# Register an Azure AD application

Time to register your own application and use that instead.

## Version 1: Azure Portal

### Step 1: Sign in to the Azure Portal

- Open a browser, go to `https://portal.azure.com`, and sign in with your Azure account. You should have gotten your username and password in the beginning of the workshop. I recommend to use a separate web browser profile for the workshop.

### Step 2: Create a new Azure AD Application

- On the left side menu, select "Azure Active Directory".
- In the Azure Active Directory blade, select "App registrations".
- Select "New registration".
- Fill in the form with the following details:
  - Name: Enter a name for your application. To avoid collisions, prefix the name with your username (e.g. "u01MyApp")
  - Supported account types: Select "Accounts in this organizational directory only (AAAAws only - Single tenant)".
  - Redirect URI: `http://localhost/demo`.
- Click on the "Register" button at the bottom.

Congratulations! You've created a new Azure AD application.

## Version 2: Azure CLI

Before you start, make sure you have the Azure CLI installed on your machine and you are logged in with an Azure account.

### Step 1: Log in to Azure

Open a terminal or command prompt, and run the following commands to log in:

```bash
az logout
az account clear
az login --tenant aaaaws.com   
```

The two first commands are to log you out of any existing subscriptions and tenants you're logged into and clear any cached accounts. This is to prevent mistakes where you're suddenly doing things in your regular tenant.

On the last line we specify the tenant we want to log into. If you're using your regular user account and your regular tenant you normally don't need to do this. Here we force it to use the workshop tenant, again so that we don't end up doing stuff in your regular tenant.

A browser window will open for you to sign in with your workshop user. After you sign in, you can close the browser and return to the terminal.

### Step 2: Create a new Azure AD Application

In the terminal, run the following command to create a new Azure AD application. Replace `<YourAppName>` with the name you want for your application:

```bash
az ad app create --display-name <YourAppName> --web-redirect-uris "http://localhost/demo" --sign-in-audience AzureADMyOrg
```

The command will return JSON output with details about the new application. The `appId` field is the application (client) ID of your new application.

Congratulations! You've created a new Azure AD application using the Azure CLI.

Make a note of the application ID, you will be using it in labs to come so it will be useful to have handy.

In both versions of the script, after creating the application, you can add credentials, configure API permissions, set redirect URIs, and make other configurations as needed by your application.

## Create a secret

In order to use the authorization code grant we need an application secret.

### Part 1: Azure Portal

1. Navigate to the Azure portal at `https://portal.azure.com`.
2. Select "Azure Active Directory" from the left-hand menu.
3. Under "Manage" in the Azure Active Directory blade, select "App registrations".
4. Select the application for which you want to add a secret credential.
5. In the application blade, select "Certificates & secrets" under "Manage".
6. In the "Certificates & secrets" blade, select "+ New client secret".
7. Enter a description for your client secret under "Description".
8. Choose a duration under "Expires". Then select "Add".
9. After you select "Add", Azure will create the client secret. Make sure to copy the value of the client secret, you won't be able to access it again.

### Part 2: Azure CLI

1. Open your terminal or command line interface.
  
2. Ensure you're logged in to your Azure account by running the following command:
  
   ```bash
   az login
   ```

   Follow the instructions to log in. If you're already logged in, you can skip this step.

3. To create a new secret for an application, run the following command (replace `{app-id}` with your application ID):
  
   ```bash
   az ad app credential reset --id {app-id} --display-name "CLI Secret" --append
   ```
  
   The `--append` flag allows you to add a new secret without deleting existing ones.

4. The command will return a JSON object containing your new client secret. The `password` field contains the value of the client secret. Make sure to save this value in a secure location, as you won't be able to retrieve it again.

In both cases, keep in mind that the client secret acts like a password for your application to authenticate against Azure AD. It is very important that you store it securely and don't share it.
