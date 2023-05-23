# Incremental consent

Create a web app that will ask for more consent as it's needed

Sure, let's start by creating an App Registration on Azure AD.

**Create an App Registration**

1. Navigate to the [Azure portal](https://portal.azure.com) and sign in with your account.
2. In the left-hand menu, click on "Azure Active Directory".
3. In the Azure Active Directory overview page, click on "App registrations".
4. Click on "New registration".
5. Enter a name for your application.
6. Leave the "Redirect URI" blank for now. We'll come back to this later.
7. Click on "Register" to create the application.

**Step 1: Create a new .NET web app**

Open a terminal or command prompt. Then create a new Web Application by running:

```bash
dotnet new webapp --auth SingleOrg  -n Incremental  --client-id {your-app-client-id} --tenant-id a1c5bee8-9e19-44cb-9f07-b94b279890ab --domain aaaaws.com
cd Incremental
```

This creates a new ASP.NET Core web app and navigates into the new project directory.

**Step 2: Upgrade Microsoft Identity Web to your app**

Next, upgrade the Microsoft Identity Web library in your app by running:

```bash
dotnet add package Microsoft.Identity.Web
dotnet add package Microsoft.Identity.Web.UI
```

**Step 3: Add EnableTokenAcquisitionToCallDownstreamApi**

In your `Program.cs`, update the  method to use the code flow and enable token acquisition:

```csharp
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(options =>
    {
        builder.Configuration.Bind("AzureAd", options);
        options.ResponseType = OpenIdConnectResponseType.Code;
    })
    .EnableTokenAcquisitionToCallDownstreamApi()
    .AddInMemoryTokenCaches();
```

Enable PII logging below the `builder.Build()` line

```csharp
var app = builder.Build();

IdentityModelEventSource.ShowPII = true;
```

During development and debugging, it can be useful to enable PII logging to get more detailed information about issues and bugs, or, in this case, a better understanding of what's going on.

Remember to turn off PII logging in production environments. Showing PII in logs can expose sensitive data and potentially violate privacy laws and regulations.

**Step 4: Add two Razor pages, Read and Write, and link them in the nav bar**

In the Pages directory, add two new Razor pages named `Read.cshtml` and `Write.cshtml`. In `_Layout.cshtml` under `div` with class `navbar-collapse`, add:

```html
<ul class="navbar-nav">
    <li class="nav-item">
        <a class="nav-link text-dark" asp-area="" asp-page="/Read">Read</a>
    </li>
    <li class="nav-item">
        <a class="nav-link text-dark" asp-area="" asp-page="/Write">Write</a>
    </li>
</ul>
```

**Step 5: Get Access Token in Read and Write Pages**

In the `ReadModel` and `WriteModel` classes of your Razor Pages, inject `ITokenAcquisition` and use `tokenAcquisition.GetAccessTokenForUserAsync`.  Add a new public property Token to store the access token:

```csharp
private readonly ITokenAcquisition _tokenAcquisition;
public string Token { get; set; } = "";

public ReadModel(ITokenAcquisition tokenAcquisition)
{
    _tokenAcquisition = tokenAcquisition;
}

public async Task OnGet()
{
    Token = await _tokenAcquisition.GetAccessTokenForUserAsync(new[] { "User.Read" });
}
```

Repeat the same for `WriteModel`, but replace `"User.Read"` with `"User.ReadWrite"`.

**Step 6: Display the Token in the Razor Pages**

In your Razor pages (Read.cshtml and Write.cshtml), you can now display the access token. Add the following lines to both pages:

```csharp
<h2>Access Token:</h2>
<p>@Model.Token</p>
```

Now, when you navigate to the Read or Write pages, you'll see the access token printed on the screen.

Please note that displaying access tokens in your views is not recommended for production applications due to security risks. This is for learning purposes only. In a real-world scenario, the access token should be used to authenticate to an API and should never be exposed to the front end.

**Step 7: Configure the App Registration**

1. Open your app registration in the portal. 
2. Click on "Authentication" in the left-hand menu.
3. In the "Platform configurations" section, click on "Add a platform" and choose "Web".
4. In the "Redirect URIs" section, enter the redirect URI for your application. This will be `https://localhost:{PORT}/signin-oidc`, where `{PORT}` is the port your application is running on.
5. In the "Front-channel logout URL" section, enter the signout URI for your application. This will be `https://localhost:{PORT}/signout-oidc` where `{PORT}` is the port your application is running on.
6. Click on "Configure" to save these settings.

**Step 8: Set API permissions**

1. In the left-hand menu, click on "API permissions".
2. Click on "Add a permission" and select "Microsoft Graph".
3. Choose "Delegated permissions" and then search for and select the "User.Read" and "User.ReadWrite" permissions.
4. Click on "Add permissions" to save these changes.
5. Now, navigate to "Certificates & secrets" in the left-hand menu.
6. In the "Client secrets" section, click on "New client secret".
7. Add a description for your client secret and choose an expiry period.
8. Click "Add" and you'll see your new client secret. Copy this value now, as you will not be able to see it again after you leave this page.

**Step 9: Update `appsettings.json` in Your Application**

Now that you've configured your app registration in Azure AD, you need to update your application with these settings. 

Open the `appsettings.json` file in your project and add the following section:

```json
"AzureAd": {
    "Instance": "https://login.microsoftonline.com/",
    "Domain": "{YOUR-DOMAIN}",
    "TenantId": "{YOUR-TENANT-ID}",
    "ClientId": "{YOUR-CLIENT-ID}",
    "CallbackPath": "/signin-oidc",
    "ClientCredentials": [
        {
            "SourceType": "ClientSecret",
            "ClientSecret": "{YOUR-CLIENT-SECRET}"
        }
    ]
}
```

Replace `{YOUR-DOMAIN}` with the domain of your Azure AD, `{YOUR-TENANT-ID}` with the "Directory (tenant) ID" from the Azure portal, and `{YOUR-CLIENT-ID}` with the "Application (client) ID" from the Azure portal.

Replace `{YOUR-CLIENT-SECRET}` with the client secret you just created.

Remember, storing sensitive data like client secrets in your source code or appsettings.json file is not recommended in a production scenario. You should use a secure secrets management system, like Azure Key Vault, for storing sensitive data.

Once this is done, your application will be able to authenticate with Azure AD using the details from the app registration.

**Step 7: Test your app**

Now, run your application with `dotnet run`. You should see that an exception is thrown when you navigate to the Read or Write page. The exception is thrown because the token acquisition fails - the user has not consented to the required scopes and we don't handle the error properly.

**Step 8: Add the AuthorizeForScopes attribute**

To properly handle these exceptions, you need to add the `[AuthorizeForScopes]` attribute to your Razor PageModel classes. This attribute will trigger a consent prompt when necessary. Update your `ReadModel` and `WriteModel` classes as follows:

```csharp
[AuthorizeForScopes(Scopes = new[] { "User.Read" })]
public class ReadModel : PageModel
{
    //...
}

[AuthorizeForScopes(Scopes = new[] { "User.ReadWrite" })]
public class WriteModel : PageModel
{
    //...
}
```

The `AuthorizeForScopes` attribute is part of the Microsoft Identity Web library and it's specifically designed to handle certain exceptions that can occur when your application tries to acquire an access token to call a protected API.

Here's what it does:

1. **Catches Exceptions**: When your application calls `ITokenAcquisition.GetAccessTokenForUserAsync`, it can throw a `MsalUiRequiredException`. This exception indicates that to get an access token, additional action must be taken by the user. This action usually means that the user needs to provide consent for the requested scopes, perform multi-factor authentication, or sign in again. The `AuthorizeForScopes` attribute will catch this exception when it occurs.

2. **Identifies Required Scopes**: The attribute has a property called `Scopes`, which you can use to specify the scopes for which your application may request a token. This information is then used to challenge the user to provide the necessary consent. 

3. **Triggers a Redirect**: When the `MsalUiRequiredException` is caught, `AuthorizeForScopes` will initiate a redirect to Azure AD, using the information in the exception and the `Scopes` property to construct the challenge. This redirect will prompt the user to complete the necessary action (e.g., provide consent, re-authenticate).

4. **Handles the Response**: After the user completes the action and returns to your application, Azure AD will include an authorization code in the response. This code can then be redeemed for an access token that includes the necessary scopes.

So, to summarize, the `AuthorizeForScopes` attribute intercepts exceptions that require user interaction, initiates the necessary user interaction by redirecting the user to Azure AD, and helps handle the response from Azure AD. By using this attribute, you can make your application more robust in handling situations where acquiring an access token requires additional user interaction.

**Step 9: Test your app again**

Before testing, make sure to sign out from your previous session to ensure that the changes take effect. Run your application again with `dotnet run`.

Once you've signed out, you can proceed with the following steps:

1. **Open the Home Page**: Navigate to the home page of your application. At this point, since you are not authenticated, you should be redirected to the Azure AD login page. After logging in you will receive an ID token that includes information about the authenticated user. Since we have enabled PII logging you should see this token in the application log output.

2. **Decode the Token**: Copy the ID token from your application's log output and navigate to [jwt.ms](https://jwt.ms). Paste your token into the input field to decode it. You'll see a detailed breakdown of your token, including the header, payload, and signature. The payload includes the claims in your token. Look for the "scp" (scopes) claim. Since you have not yet visited the Read or Write pages and consented to the respective scopes, this claim should not be present.

3. **Open the Read Page**: Now, navigate to the Read page in your application. This should prompt a consent screen for the `User.Read` scope if you haven't consented to it already. After consenting, your application will acquire an access token for this scope. Again, copy the token and decode it on jwt.ms. This time, the "scp" claim should include `User.Read`.

4. **Open the Write Page**: Finally, navigate to the Write page. This will prompt a consent screen for the `User.ReadWrite` scope, provided you haven't consented to it already. After consenting, your application will acquire an access token for this scope. Copy this token and decode it on jwt.ms. The "scp" claim should now include both `User.Read` and `User.ReadWrite`.

As you progress through these steps, you will see the "scp" claim in your access token expand to include more scopes as you consent to them. This is a demonstration of incremental and dynamic consent in action. With this feature, your application only requests the permissions it needs when it needs them, improving the user experience and adhering to the principle of least privilege.
