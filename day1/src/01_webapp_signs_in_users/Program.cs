using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddRazorPages()
  // Add controllers and pages in the /MicrosoftIdentity path that has sign-in and sign-out pages == Signin/out frontends
  .AddMicrosoftIdentityUI();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
// If static file, return file and skip remaining middleware
app.UseStaticFiles();

// Find the controller/page
app.UseRouting();
// Enables the authorization setup created by AddAuthorization, which uses AddAuthentication, which uses the appsettings.json -> AzureAd block
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

app.Run();
