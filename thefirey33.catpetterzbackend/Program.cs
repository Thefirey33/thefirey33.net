using Microsoft.AspNetCore.WebSockets;
using Scalar.AspNetCore;
using thefirey33.catpetterzBackend.AuthenticationHandler;
using thefirey33.catpetterzBackend.Service;
using thefirey33.catpetterzBackend.Types.Database;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddHttpLogging();
builder.Services.AddLogging();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddWebSockets(options => { options.KeepAliveInterval = TimeSpan.FromSeconds(10); });
builder.Services.AddRouting();
builder.Services.AddAntiforgery();
builder.Services.AddControllers();
builder.Services.AddControllersWithViews();
builder.Services.AddOpenApi();
builder.Services.AddSignalR();

// Add the database context.
builder.AddMongoDbContext<CatPetterzDbContext>("catpetterzdb", "catpetterzdb");

builder.Services.AddHttpClient("FilteringServiceAPI",
    options => { options.BaseAddress = new Uri("https+http://fireyfilteringservice"); });

// Add the cat state update service.
builder.Services.AddHostedService<UpdateCatStateService>();

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
app.UseWebSockets();

using (var scope = app.Services.CreateScope())
{
    // Ensure that the database is created
    // Sİnce MongoDB doesn't have migration, this is our best bet.

    var catpetterzDbContext = scope.ServiceProvider.GetRequiredService<CatPetterzDbContext>();
    await catpetterzDbContext.Database.EnsureCreatedAsync();
}

app.Run();