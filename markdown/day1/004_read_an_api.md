# Have your app read another API

Let's have our app read another API. 

## Prerequisites

- Application registered with Azure AD (from previous lab)
- Application that signs in a user (from previous lab)
- Application ID from that application
- Application Client ID (**not the same as the ID**)
- Application secret (from previous lab)
- Your tenant ID (you can find it by running `az account show`)

Open the starting project in the `02_webapp_accesses_graph` folder.

## Part 1: Update your application settings

Now that we'll be calling another API, we need to make our application a so-called confidential client. That means it needs it's client secret. For this lab's purpose, we'll add that in the `appsettings.json` file. This is of course really bad security wise, but it makes the lab simpler. We'll talk about some other ways of securing this information. 

In your `appsettings.json` file, update the `AzureAD` section to include a `ClientCredentials` value:

```json
        "ClientCredentials": [
            {
                "SourceType": "ClientSecret",
                "ClientSecret": "{your-client-secret}"
            }
        ]
```

Add another top-level section, describing the API you'll be calling and what scopes you'll be using.

```json
    "GraphBeta": {
        "BaseUrl": "https://graph.microsoft.com/beta",
        "Scopes": "user.readbasic.all"
    },
```


## Part 2: Update your application startup

We need to register some more services with the middleware. This time for acquiring tokens and storing them. The last three lines are new, add those. 

```csharp
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"))
        .EnableTokenAcquisitionToCallDownstreamApi(new[] { "user.readbasic.all" })
            .AddMicrosoftGraph(builder.Configuration.GetSection("GraphBeta"))
            .AddInMemoryTokenCaches();

```

This registers the middleware for the `GraphServiceClient` that make it easier to use the Microsoft Graph API. It also registers a token acquisition service (which is used by the `GraphServiceClient`) and a token cache to keep the tokens around.

In the `Pages/Users.cshtml.cs` file, you can see how the `GraphServiceClient` is used to fetch the user list. All the token acquisition is handled my the `Microsoft.Identity.Web` library.
