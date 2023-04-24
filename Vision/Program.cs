using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.Extensions.Hosting;
using Vision.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Vision.Services;
using Azure.Identity;
using Vision.Hubs;
using Microsoft.AspNetCore.ResponseCompression;
using MudBlazor.Services;
using Newtonsoft.Json;
using Synlogic.Azure.KeyVault;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationInsightsTelemetry(builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]);


var keyVaultEndpoint = new Uri(builder.Configuration.GetValue<string>("Azure:VaultUri"));

var tenant = builder.Configuration.GetValue<string>("AzureAd:TenantId");
var client = builder.Configuration.GetValue<string>("AzureAd:ClientId");
var secret = builder.Configuration.GetValue<string>("AzureAd:ClientSecret");
ClientSecretCredential credentials = new ClientSecretCredential(tenant, client, secret);
builder.Configuration.AddAzureKeyVault(keyVaultEndpoint, credentials);

#if DEBUG
bool debug = true;
#else
bool debug = false;
#endif


builder.Services.AddSignalR().AddAzureSignalR(options =>
{
    options.ServerStickyMode = Microsoft.Azure.SignalR.ServerStickyMode.Required;
});

// Add services to the container.
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"));
builder.Services.AddControllersWithViews() //.AddJsonOptions(o => o.JsonSerializerOptions.ReferenceHandler.) RPS op deze manier is er ook een oplossing te vinden voor het circulaire probleem van json serialisatie, maar ben er nog niet helemaal uit.
    .AddMicrosoftIdentityUI();
//builder.Services.AddControllers().AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

builder.Services.AddAuthorization(options =>
{
    // By default, all incoming requests will be authorized according to the default policy
    options.FallbackPolicy = options.DefaultPolicy;
});

//EnvironmentTagHelper.Init(Configuration["VaultUri"], Configuration["AzureAd:TenantId"], Configuration["AzureAd:ClientId"], Configuration["AzureAd:ClientSecret"]);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor()
    .AddHubOptions(options =>
    {
        options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
        options.EnableDetailedErrors = true;
        options.HandshakeTimeout = TimeSpan.FromSeconds(15);
        options.KeepAliveInterval = TimeSpan.FromSeconds(15);
        options.MaximumParallelInvocationsPerClient = 1;
        options.MaximumReceiveMessageSize = 32 * 1024;
        options.StreamBufferCapacity = 10;
    }).AddMicrosoftIdentityConsentHandler();
builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        new[] { "application/octet-stream" });
});
//
#if DEBUG

builder.Services.AddDbContext<SynscrumxxlDevSqlContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("SynScrumNEW_DEV")), ServiceLifetime.Singleton);
builder.Services.AddDbContextFactory<SynscrumxxlDevSqlContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("SynScrumNEW_DEV")), ServiceLifetime.Singleton);
#else
builder.Services.AddDbContext<SynscrumxxlDevSqlContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("SynScrumNEW_PROD")).UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking), ServiceLifetime.Singleton);
builder.Services.AddDbContextFactory<SynscrumxxlDevSqlContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("SynScrumNEW_PROD")).UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking), ServiceLifetime.Singleton);
//string conn = builder.Configuration.GetConnectionString("SynScrumXXLDB_PROD");
#endif
builder.Services.AddSingleton<DataContextProvider>();

builder.Services.AddSingleton<EventGridService>();

builder.Services.AddScoped<PersonalContextProvider>();

builder.Services.AddScoped<TimeEntryService>();
builder.Services.AddMudServices();


//prod

Synlogic.Azure.KeyVault.KeyVault.Init(
builder.Configuration.GetValue<string>("Azure:VaultUri"),
builder.Configuration.GetValue<string>("AzureAd:TenantId"),
builder.Configuration.GetValue<string>("AzureAd:ClientId"),
builder.Configuration.GetValue<string>("AzureAd:ClientSecret"));

string atUsername = ((debug) ? "Sandbox" : "") + "AtUsername";
string atPassword = ((debug) ? "Sandbox" : "") + "AtPassword";
string atApiIntegrationCode = ((debug) ? "Sandbox" : "") + "AtApiIntegrationCode";

//AutoTaskCredentials.Instance(KeyVault.GetSecret(atUsername), KeyVault.GetSecret(atPassword), KeyVault.GetSecret(atApiIntegrationCode));
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd")).EnableTokenAcquisitionToCallDownstreamApi()
            .AddMicrosoftGraph(builder.Configuration.GetSection("MicrosoftGraph"))
            .AddInMemoryTokenCaches();
var app = builder.Build();

app.UseResponseCompression();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapBlazorHub();
app.MapHub<EventGridHub>("sync");
app.MapFallbackToPage("/_Host");



app.Run();
