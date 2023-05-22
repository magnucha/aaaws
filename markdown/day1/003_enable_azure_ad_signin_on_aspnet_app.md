# Enable Azure AD sign-in on an ASP.NET application

Let's modify an existing ASP.NET Core web app to enable user sign-in and show the user name in the heading.

## Prerequisites

- Application registered with Azure AD (from previous lab)
- Application ID from that application
- Application Client ID (**not the same as the ID**)
- Your tenant ID (you can find it by running `az account show`)

## Part 1: Update Application Registration in Azure AD

For this lab we update our application registration to allow for a new authentication flow, the Implicit grant. This allows our app to get a token without needing the application secret. One less secret to manage!

1. Log in to the Azure portal.
2. Navigate to Azure Active Directory > App Registrations.
3. Click on your registered application.
4. Go to Authentication.
5. Under the Implicit grant and hybrid flows, check the box for Access ID tokens. This will enable implicit and hybrid flow.
6. Click Save.

You can also do this from the terminal:

```bash
az ad app update --id {application-id} --enable-id-token-issuance true
```

We also need to update the redirect URLs for the app. In the portal, on your application: 

1. Go to Authentication.
2. Under Web, click `Add URI`
3. Add `https://localhost:7014/signin-oidc`
4. Repeat and add `https://localhost:44336/signin-oidc` if you'll be running the app in IIS Express.
5. Under Front-channel logout URL, put `https://localhost:7014/signout-oidc` (or port `44321` if you're using IIS Express)


This part is a bit more convoluted on the command line since the CLI does not support updating values on sub-levels (`web.redirectURIs`). But we can use the Microsoft Graph API which is always available, although not as intuitive always. The `az` CLI has a command, `rest`, that automatically includes authentication when calling certain Microsoft APIs, including the Graph API.

This command will update the redirect URIs on your application. You need to replace `{application-id}` with your app id. Note that this will overwrite the existing redirect URI. If you want to keep that, include it in the array or add it in the portal afterwards. We don't need it any more in the labs so it doesn't matter that you overwrite it.

```bash
az rest --method PATCH --uri 'https://graph.microsoft.com/v1.0/applications/{application-id}' --body "{'web':{'redirectUris':['https://localhost:7014/signin-oidc', 'https://localhost:44336/signin-oidc']}}"
```

To add the logout URI: 

```bash
az rest --method PATCH --uri 'https://graph.microsoft.com/v1.0/applications/{application-id}' --body "{'web':{'logoutUrl':'https://localhost:7014/signout-oidc'}}"
```

## Part 2: Modify ASP.NET Core web app

A web app is located in the `/day1/src/01_webapp_signs_in_users` folder. Open that in your preferred editor and make the following changes: 

1. Install the `Microsoft.Identity.Web` and the `Microsoft.Identity.Web,UI` NuGet packages.
  
   ```bash
   dotnet add package Microsoft.Identity.Web
   dotnet add package Microsoft.Identity.Web.UI
   ```

2. Update the `appsettings.json` file with Azure AD configuration values:

   ```json
   "AzureAd": {
     "Instance": "https://login.microsoftonline.com/",
     "Domain": "aaaaws.com",
     "TenantId": "{your-tenant-id}",
     "ClientId": "{application-client-id}",
     "CallbackPath": "/signin-oidc"
   }
   ```

3. Modify the `Program.cs` file:
   - At the top of the file, add these using statements:

     ```csharp
     using Microsoft.Identity.Web;
     using Microsoft.Identity.Web.UI;
     ```

   - After the `var builder = WebApplication.CreateBuilder(args);` line, add the following code to enable authentication services:

     ```csharp
     // Register the authentication middleware
     builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
       .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"));

     builder.Services.AddAuthorization(options =>
     {
         // By default, all incoming requests will be authorized 
         // according to the default policy, which requires an 
         // authenticated user
         options.FallbackPolicy = options.DefaultPolicy;
     });
     ```

   - Between the `app.UseRouting();` and `app.MapRazorPages();` lines, add the authorization middleware:

     ```csharp
     app.UseAuthorization();
     ```

4. In the `Pages/Shares/_Layout.cshtml` file, uncomment the LoginPartial:

   From:

   ```html
   @* <partial name="_LoginPartial" /> *@
   ```

   To:

   ```html
   <partial name="_LoginPartial" />
   ```

5. Take a look at the `Pages/Shared/_LoginPartial.cshtml` and note how it's using the `User.Identity?.IsAuthenticated` value, which is populated automatically by the middleware.

6. Run the app, sign in with your Azure AD account, and verify that the user's name appears in the heading. Try to sign out and sign back in again.

Remember to replace placeholders (`{your-tenant-id}`, `{application-client-id}`) with actual values.

## Part 3: See the user's claims

In the `Pages/Index.cshtml` file, add the following to the bottom of the file:

```csharp
<div>
    @foreach(var claim in User.Claims) 
    {
        <p>@claim.Type: @claim.Value</p>
    }
</div>
```

Reload the app and view the front page. You should now see the claims that the authentication middleware has automatically populated your identity object with.
