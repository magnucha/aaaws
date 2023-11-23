using Microsoft.Identity.Client;

var builder = PublicClientApplicationBuilder.Create("3e5ec436-1919-47a8-8f26-e310dec13af5")
    .WithAuthority(AzureCloudInstance.AzurePublic, "a1c5bee8-9e19-44cb-9f07-b94b279890ab")
    .WithRedirectUri("http://localhost")
    .WithCacheOptions(new CacheOptions());


var app = builder.Build();

var result = await app.AcquireTokenWithDeviceCode(new[] { "User.Read.All" }, deviceCodeResult =>
    {
        Console.WriteLine(deviceCodeResult.Message);
        return Task.CompletedTask;
    }).ExecuteAsync();


Console.WriteLine("Access Token: " + result.AccessToken);
