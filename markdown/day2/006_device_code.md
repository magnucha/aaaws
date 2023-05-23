# Device code flow

A different way of authenticating, very useful for devices that don't have a web browser available. 

**Instructions:**

**Step 1: Create an Azure AD app registration**

Create a new Azure AD app registration (replace `<app-name>` with your preferred name):

```bash
az ad app create --display-name <app-name>
```

Take note of the `appId` in the output as you'll need this later.

Grant it application permissions to the User.Read.All scope and give it admin consent.

**Step 3: Create a console application**

3.1. Create a new .NET console application (replace `<app-name>` with your preferred name):

```bash
dotnet new console -n <app-name>
```

3.2. Add the `Microsoft.Identity.Client` NuGet package to the project:

```bash
cd <app-name>
dotnet add package Microsoft.Identity.Client
```

3.3. Open `Program.cs` in your preferred editor and replace the existing code with the following:

```csharp
using Microsoft.Identity.Client;

var app = PublicClientApplicationBuilder.Create("<app-id>")
    .WithAuthority(AzureCloudInstance.AzurePublic, "a1c5bee8-9e19-44cb-9f07-b94b279890ab")
    .WithRedirectUri("http://localhost")
    .Build();

var result = await app.AcquireTokenWithDeviceCode(new[] { "User.Read.All" }, deviceCodeResult =>
    {
        Console.WriteLine(deviceCodeResult.Message);
        return Task.CompletedTask;
    }).ExecuteAsync();

Console.WriteLine("Access Token: " + result.AccessToken);
```

Make sure to replace `<app-id>` with your app ID.

3.4. Run the console application:

```bash
dotnet run
```

You should see a device code and a URL. Open the URL in a browser, enter the device code, and sign in with your Azure AD account. The console application will then print the access token.

This completes the lab. You have created an Azure AD app registration, assigned it permissions to read all users via the Graph API, and created a console application that uses Microsoft.Identity and the device code flow.

*Note: Please