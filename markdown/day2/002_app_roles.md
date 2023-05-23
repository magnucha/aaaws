# Experimenting with Azure AD App roles. 

We will create an app registration, define app roles, and create an ASP.NET web application to enforce these roles.

**Step 1: Create an App Registration**

1. Go to the [Azure portal](https://portal.azure.com/).
2. Navigate to Azure Active Directory > App registrations.
3. Click on "New registration".
4. Enter a name for your app, choose the supported account types, and click "Register".
5. Create a Secret for the app.

**Step 2: Define App Roles**

1. In your new app registration, navigate to "App Roles".
2. Click "+ Create App role"
3. Use "User" for Display name, Value and Description.
4. Allowed member types should be "Users/Groups".
5. Click Apply.
6. Repeat for the app role "PowerUsers".

**Step 3: Create a New ASP.NET Core Web App**

1. Open a terminal and create a new ASP.NET Core web application.

    ```bash
    dotnet new webapp --auth SingleOrg -n Incremental --client-id 664253e9-9bb3-47a6-a33d-7da960e7bd58 --tenant-id a1c5bee8-9e19-44cb-9f07-b94b279890ab --domain aaaaws.com
    cd Incremental
    ```

2. Update the Identity NuGet packages:

    ```bash
    dotnet add package Microsoft.Identity.Web
    dotnet add package Microsoft.Identity.Web.UI
    ```

3. Update your `Program.cs` and `appsettings.json` as described in the previous lab to configure the web app with your app registration details.

4. Update  `Program.cs` to use the auth flow:

    ```csharp
    builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
        .AddMicrosoftIdentityWebApp(options =>
        {
            builder.Configuration.Bind("AzureAd", options);
            options.ResponseType = OpenIdConnectResponseType.Code;
        });
    ```

**Step 4: Define Authorization Policy**

In `Program.cs` enforce the `User` role as a fallback policy. Replace `options.FallbackPolicy = options.DefaultPolicy;` with:

```csharp
 options.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().RequireRole("User").Build();
```

**Step 5: Update Home Page**

In `Pages/Index.cshtml`, list the claims of the logged-in user. At the bottom of the page, add:

```html
<h2>Claims</h2>
<ul>
@foreach(var claim in User.Claims) 
{
    <li>@claim.Type: @claim.Value</li>
}
</ul>

```

**Step 6: Create a PowerUser Page**

Create a new Razor page `PowerUser.cshtml` and `PowerUser.cshtml.cs` similar to the home page but this page is only accessible to users with the PowerUser role:

```csharp
[Authorize(Roles = "PowerUser")]
public class PowerUserModel : PageModel
{
    ...
}
```

Update your layout or navigation component to include a link to this new page.

**Step 7: Configure the App Registration**

1. Open your app registration in the portal. 
2. Click on "Authentication" in the left-hand menu.
3. In the "Platform configurations" section, click on "Add a platform" and choose "Web".
4. In the "Redirect URIs" section, enter the redirect URI for your application. This will be `https://localhost:{PORT}/signin-oidc`, where `{PORT}` is the port your application is running on.
5. In the "Front-channel logout URL" section, enter the signout URI for your application. This will be `https://localhost:{PORT}/signout-oidc` where `{PORT}` is the port your application is running on.
6. Click on "Configure" to save these settings.

**Step 8: Testing**

Run your application, and log in with a user who has not been assigned any roles. You should not be able to access the home page

**Step 9: Assign the User role**

Assigning a user to an application role in your app registration can be done via the Azure portal. Here's how:

1. **Select your app registration**: Find and open your application registration.

2. **Open the managed application** On the overview page, click the managed application link. 

3. **Go to "Users and groups"**: On your app registration's menu, click on "Users and groups". 

4. **Add user to app role**: On the "Users and groups" pane, click on "+ Add user/group", then click "None selected" under "Users" on pane that appears.

5. **Select the user and role**: On the "Users" pane, select the user you want to add the role to, and click on the "Select" button at the bottom. Back on the "Add Assignment" pane, select "Roles", pick the "User" role, and click on the "Select" button at the bottom.

6. **Complete the assignment**: Back on the "Add Assignment" pane, click on the "Assign" button at the bottom to complete the assignment.

Now, the selected user is assigned to the app role. Sign out and then back into the application.

**Step 10: Testing more**

Having signed out and back in again, you should be able to see the homepage. Notice that you have the `User` role claim. 

**Step 11: Hide the PowerUser link**

Good UX design says we should not show options that the user is not able to use. Let's hide the PowerUser link. 

In `_Layout.cshtml` only show the PowerUser link if the user has the proper role: 

```html
@if (User.IsInRole("PowerUser"))
{
    <li class="nav-item">
        <a class="nav-link text-dark" asp-area="" asp-page="/PowerUser">PowerUser</a>
    </li>
}
```

Reload your app and verify that the link no longer appears on the web page.

**Step 12: Add the PowerUser role to your user** 

Repeat Step 9, but for the PowerUser role. 

**Step 13: Test again** 

Sign out and sign back in again. The PowerUser link should now appear and you should have both the `User` and `PowerUser` claim in your claim list.

