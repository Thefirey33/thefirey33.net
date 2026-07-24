using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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

builder.Services.AddHostedService<GitService>();
builder.Services.AddSingleton<DataService>();

// Add database context.
builder.AddNpgsqlDbContext<ArtsContext>("artdb");
builder.AddNpgsqlDbContext<ScoreContext>("scoredb");
builder.AddNpgsqlDbContext<NikoDexRecoveryContext>("nikodexdb");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
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

builder.Services
    .AddAuthorization()
    .AddControllers();

// Add the HTTP Client for the communication with other APIs out there. 
builder.Services.AddHttpClient("GitHubAPI", client => { client.BaseAddress = new Uri("https://api.github.com"); });

// Add the Redis Client for caching.
builder.AddRedisClient("fireycache");
builder.AddRedisOutputCache("fireycache");

builder
    .Services.AddCors(options =>
    {
        // Add the cors policy for the frontend.
        options.AddPolicy("AllowSpecificOrigin",
            policy => policy.WithOrigins(Environment.GetEnvironmentVariable("FIREYFRONTEND_HTTP")
                                         ?? throw new NullReferenceException("Frontend URL not specified!"))
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowAnyOrigin());
    })
    .AddRouting();

var app = builder.Build();

app.MapDefaultEndpoints();

app.UseHttpsRedirection();
app.UseAntiforgery();
app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();
app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseRouting();
app.UseCors();
app.UseHsts();
app.UseOutputCache();

using (var scope = app.Services.CreateScope())
{
    // Migrate the art DB to the latest.
    var artDb = scope.ServiceProvider.GetRequiredService<ArtsContext>();
    await artDb.Database.MigrateAsync();

    // Migrate to score DB to the latest.
    var scoreDb = scope.ServiceProvider.GetRequiredService<ScoreContext>();
    await scoreDb.Database.MigrateAsync();

    // Migrate the NikoDex DB to the latest.
    var nikoDexDb = scope.ServiceProvider.GetRequiredService<NikoDexRecoveryContext>();
    await nikoDexDb.Database.MigrateAsync();
}

app.Run();