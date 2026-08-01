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

builder.Services
    .AddEndpointsApiExplorer()
    .AddAntiforgery()
    .AddOpenApi()
    .AddAntiforgery(options =>
    {
        options.HeaderName = "X-CSRF-TOKEN";
        options.Cookie.Name = "X-CSRF-TOKEN";
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.HttpOnly = false;
    });

builder.Services.AddSingleton<DataService>();
builder.Services.AddScoped<IDexDataService, DexDataService>();
builder.Services.AddSingleton<IAuthorizationCodeService, AuthorizationCodeService>();

// Add database context.
builder.AddNpgsqlDbContext<ArtsContext>("artdb");
builder.AddNpgsqlDbContext<NikoDexRecoveryContext>("nikodexdb");
builder.AddNpgsqlDbContext<ApprovalContext>("approvaldb");

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

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var authHeader = context.Request.Headers.Authorization.ToString();
                if (!string.IsNullOrEmpty(authHeader) &&
                    authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                    context.Token = authHeader["Bearer ".Length..].Trim();

                if (!string.IsNullOrEmpty(context.Token)) return Task.CompletedTask;
                var rawCookieHeader = context.Request.Headers.Cookie.ToString();

                if (string.IsNullOrEmpty(rawCookieHeader)) return Task.CompletedTask;
                var tokenCookie = rawCookieHeader.Split(';')
                    .Select(c => c.Trim())
                    .FirstOrDefault(c => c.StartsWith("Token=", StringComparison.OrdinalIgnoreCase));

                if (tokenCookie != null) context.Token = tokenCookie["Token=".Length..];

                return Task.CompletedTask;
            }
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
    // Perform all the necessary migrations.
    var artDb = scope.ServiceProvider.GetRequiredService<ArtsContext>();
    var nikoDexDb = scope.ServiceProvider.GetRequiredService<NikoDexRecoveryContext>();
    var approvalDb = scope.ServiceProvider.GetRequiredService<ApprovalContext>();

    await artDb.Database.MigrateAsync();
    await nikoDexDb.Database.MigrateAsync();
    await approvalDb.Database.MigrateAsync();
}

app.Run();