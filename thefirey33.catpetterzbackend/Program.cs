using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using thefirey33.catpetterzBackend.AuthenticationHandler;
using thefirey33.catpetterzBackend.Types.Database;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddHttpLogging();
builder.Services.AddLogging();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddRouting();
builder.Services.AddAntiforgery();
builder.Services.AddControllers();
builder.Services.AddControllersWithViews();
builder.Services.AddOpenApi();

// Add the database context.
builder.AddNpgsqlDbContext<CatpetterzDbContext>("catpetterz");

builder.Services.AddHttpClient("FilteringServiceAPI",
    options => { options.BaseAddress = new Uri("https+http://fireyfilteringservice"); });

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = DiscordAuthenticationOptions.DefaultScheme;
    })
    .AddScheme<DiscordAuthenticationOptions, DiscordAuthenticationChallengeHandler>(
        DiscordAuthenticationOptions.DefaultScheme,
        null);
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapDefaultEndpoints();
app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpLogging();
app.UseAntiforgery();
app.UseHttpsRedirection();

using (var scope = app.Services.CreateScope())
{
    // This will migrate the specified database.
    var dbContext = scope.ServiceProvider.GetRequiredService<CatpetterzDbContext>();
    await dbContext.Database.MigrateAsync();
}

app.Run();