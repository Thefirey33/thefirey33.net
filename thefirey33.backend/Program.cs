using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using thefirey33_backend.Services;
using thefirey33_backend.Types.Database.Context;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAntiforgery();
builder.Services.AddOpenApi();

builder.Services.AddScoped<IDexDataService, DexDataService>();

builder.Services.AddSingleton<DataService>();
builder.Services.AddSingleton<IAuthorizationCodeService, AuthorizationCodeService>();

// Arts Database
builder.AddNpgsqlDbContext<ArtsContext>("artdb");

// NikoDex Recovery Service Database
builder.AddNpgsqlDbContext<NikoDexRecoveryContext>("nikodexdb");

// Approval Database
builder.AddNpgsqlDbContext<ApprovalContext>("approvaldb");

// Question Database
builder.AddNpgsqlDbContext<QuestionContext>("questiondb");

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    // Clear restrictions so it accepts proxy headers from localhost/Cloudflare Tunnel
    options.KnownIPNetworks.Clear();
    options.KnownProxies.Clear();
});


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"] ??
                          throw new NullReferenceException("JWT Issuer not found!"),
            ValidAudience = builder.Configuration["Jwt:Audience"] ??
                            throw new NullReferenceException("JWT Audience not found!"),
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ??
                                                                               throw new NullReferenceException(
                                                                                   "JWT Key must be provided!")))
        };
    });


builder.Logging
    .ClearProviders()
    .AddConsole();

builder.Services.AddAuthorization();
builder.Services.AddControllers();

// Add the HTTP Client for the communication with other APIs out there. 
builder.Services
    .AddHttpClient("GitHubAPI", client => { client.BaseAddress = new Uri("https://api.github.com"); });

builder.Services
    .AddHttpClient("NikoDexAPI", client =>
    {
        client.BaseAddress = new Uri("https://nikodex.net/api/");

        // Explicitly define the user agent so it's easy to spot.
        client.DefaultRequestHeaders.UserAgent.Add(ProductInfoHeaderValue.Parse("Thefirey33NikoDexBackupService"));
    });

// This is for the Python Backend portion that checks for innapropriate content.
// It's also the backend HTTP Connection for the Mad Mew Mew Bot.
builder.Services.AddHttpClient("FilteringServiceAPI",
    client => { client.BaseAddress = new Uri("https+http://fireyfilteringservice"); });

// Add the Redis Client for caching.
builder.AddRedisClient("fireycache");
builder.AddRedisOutputCache("fireycache");

// Add the NikoDex Recovery Service for the backups.
builder.Services.AddHostedService<NikoDexRecoveryService>();
builder.Services.AddRouting();

var app = builder.Build();

app.UseRouting();
app.MapDefaultEndpoints();
app.UseHttpsRedirection();
app.UseAntiforgery();
app.UseForwardedHeaders();
app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();
app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseCors();
app.UseHsts();
app.UseOutputCache();


using (var scope = app.Services.CreateScope())
{
    var artDb = scope.ServiceProvider.GetRequiredService<ArtsContext>(); // Arts Migration
    await artDb.Database.MigrateAsync();

    var nikoDexDb =
        scope.ServiceProvider.GetRequiredService<NikoDexRecoveryContext>(); // NikoDex Recovery Service Migration
    await nikoDexDb.Database.MigrateAsync();

    var approvalDb =
        scope.ServiceProvider
            .GetRequiredService<ApprovalContext>(); // Approval Service (for the Minecraft Server) Migration
    await approvalDb.Database.MigrateAsync();

    var questionDb =
        scope.ServiceProvider.GetRequiredService<QuestionContext>(); // The Migration for the Question system.
    await questionDb.Database.MigrateAsync();
}

app.Run();