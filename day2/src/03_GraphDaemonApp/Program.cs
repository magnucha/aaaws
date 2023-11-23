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