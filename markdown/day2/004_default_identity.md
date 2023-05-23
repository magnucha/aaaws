# Using the default identity 

Here's a way to tap into the default identity in whatever environment you're running in

1. **Create a new ASP.NET Core project with no identity**

Use the following command to create a new ASP.NET Core project named "DefaultIdentity" no authentcation:

```bash
dotnet new webapp -n DefaultIdentity
```

1. **Navigate into the project directory**

```bash
cd DefaultIdentity
```

3. **Add the NuGet packages**

Add the `Azure.Identity` and `Microsoft.Graph` NuGet packages to the project using the following commands:

```bash
dotnet add package Azure.Identity
dotnet add package Microsoft.Graph
```

4. **Modify the `OnGet` method in `Index.cshtml.cs`**

Open the `Pages/Index.cshtml.cs` file and modify the `OnGet` method to be an asynchronous method that retrieves a list of users from Azure AD using the Microsoft Graph API.

The `IndexModel` class should look something like this:

```csharp
public class IndexModel : PageModel
{
    public IList<User> Users { get; set; } = new();

    public async Task OnGetAsync()
    {
        var credential = new DefaultAzureCredential();
        var graphClient = new GraphServiceClient(credential);

        var result = await graphClient.Users.GetAsync();
        if (result?.Value != null)
        {
            Users = result.Value;
        }
    }
}
```

5. **Modify the `Index.cshtml` file**

In the `Pages/Index.cshtml` file, add a table to list the names and usernames of all users:

```html
@page
@model IndexModel
@{
    ViewData["Title"] = "Home page";
}

<div class="text-center">
    <h1 class="display-4">Welcome</h1>
    <table class="table">
        <thead>
            <tr>
                <th>Name</th>
                <th>Username</th>
            </tr>
        </thead>
        <tbody>
            @foreach (var user in Model.Users)
            {
                <tr>
                    <td>@user.DisplayName</td>
                    <td>@user.UserPrincipalName</td>
                </tr>
            }
        </tbody>
    </table>
</div>
```

The `DefaultAzureCredential` class is a part of the Azure.Identity library and it provides a simplified way to obtain a token from the Azure Active Directory service, which can then be used to authenticate with Microsoft Graph or other services. The `DefaultAzureCredential` attempts to authenticate through multiple methods, including environment variables, managed identity, and the Azure CLI. If the application is running in an Azure environment with managed identity enabled, it will use that. Otherwise, it can use a service principal specified by environment variables, or it can use the account that's logged into the Azure CLI.

Please note that the provided code sample may not work as expected because it misses some important details such as setting the correct scope when creating the `GraphServiceClient` instance and `DefaultAzureCredential` options, it is also skipping error handling which is not a good practice for real-world application. To get it to work with your Azure AD tenant, you will need to configure the required permissions for the application in Azure AD, and ensure the appropriate settings are available to `DefaultAzureCredential` at runtime.