using Aspire.Hosting.Docker.Resources.ServiceNodes;
using Microsoft.Extensions.Hosting;
using Projects;
using Scalar.Aspire;

var builder = DistributedApplication.CreateBuilder(args);

#pragma warning disable ASPIREJAVASCRIPT001
#pragma warning disable ASPIREDOCKERFILEBUILDER001
#pragma warning disable ASPIREPERSISTENCE001

var compose =
    builder.AddDockerComposeEnvironment("compose");

// The admin username of the admin interface.
var adminUsername = builder.AddParameter("admin-username", true);

// The admin password of the admin interface.
var adminPassword = builder.AddParameter("admin-password", true);

// The operator parameter.
// Basically defines the operator of the Minecraft server.
var trustedOperatorUuid = builder.AddParameter("trusted-operator-uuid");

// The caching/redis service for the backend.
var redis
    = builder.AddRedis("fireycache")
        .WithDataVolume(isReadOnly: false)
        .WithPersistence()
        .WithRedisInsight();


// The PostgresSQL database.
// Will be used for the forums and NikoDex backups.
var postgresSql
    = builder.AddPostgres("fireydatabase")
        .WithPassword(builder.AddParameter("postgres-password", true))
        .WithDataVolume()
        .WithPgAdmin();

// This is where the backups of the NikoDex are stored.
// Every month, the backend will request to the NikoDex and will store a backup of the Dex.
// With this, the Dex, if in case of emergency, will have a backup to go to.
var nikoDexBackupDb = postgresSql.AddDatabase("nikodexdb");

// This is where all the arts that were made for Thefirey33, or by me will be uploaded.
var artPostingDb = postgresSql.AddDatabase("artdb");

// This is for the advanced whitelisting system in the Minecraft server.
// Each joining user will require approval.
var approvalDb = postgresSql.AddDatabase("approvaldb");

// This is for the Questions that can be asked on the website.
// It will require Discord Authentication.
var questionDb = postgresSql.AddDatabase("questiondb");

// The Question System is managed by two services,
// The Discord Authentication Service and Website's Backend Itself.

// The Scalar API reference.
var scalar = builder.AddScalarApiReference();


// This is the filtering service.
// For filtering content sent by the user.
var filteringService = builder
    .AddUvicornApp("fireyfilteringservice", "../thefirey33.contentfilter", "main:app")
    .WithDockerfileBaseImage("python:3.11.15-trixie", "python:3.11.15-trixie")
    .WithEnvironment("CLIENT_ID", builder.AddParameter("bot-client-id", true))
    .WithEnvironment("CLIENT_SECRET", builder.AddParameter("bot-client-secret", true))
    .WithEnvironment("REDIRECT_URI", builder.AddParameter("bot-redirect-uri"))
    .WithEnvironment("BOT_TOKEN", builder.AddParameter("bot-token", true))
    .WithHttpHealthCheck("/health")
    .WithHttpEndpoint(env: "PORT");

// If it's the development environment, do not attempt to create a Cloudflare WARP Service.
if (!builder.Environment.IsDevelopment())
{
    var cloudflareWarpService = builder.AddContainer("fireywarp", "caomingjun/warp")
        .WithBindMount("/var/lib/cloudflare-warp", "/data")
        .PublishAsDockerComposeService((_, service) =>
        {
            service.User =
                "0:0"; // The Cloudflare WARP Service needs to run as ROOT in order to be able to edit the interfaces.
            service.CapAdd = ["NET_ADMIN"];
            service.Restart = "unless-stopped";
            service.Ports = ["1080:1080"];
            service.Sysctls = new Dictionary<string, string>
            {
                { "net.ipv6.conf.all.disable_ipv6", "0" },
                { "net.ipv4.conf.all.src_valid_mark", "1" },
                { "net.ipv4.ip_forward", "1" },
                { "net.ipv6.conf.all.forwarding", "1" },
                { "net.ipv6.conf.all.accept_ra", "2" }
            };
        })
        .WithHttpEndpoint(1080, 1080, "proxy");

    filteringService.WithEnvironment("PROXY", cloudflareWarpService.GetEndpoint("proxy"));
}


// This is the Minecraft Server.
// Managed by the FireServer Minecraft Plugin.
var backend =
    builder.AddProject<thefirey33_backend>("fireybackend")
        .PublishAsDockerComposeService((_, service) =>
        {
            service.Name = "fireybackend";
            service.User = "0:0"; // Unfortunately, some things just don't turn out how they're supposed to be.

            service.AddVolume(new Volume
            {
                Type = "volume",
                Name = "fireybackend-volume",
                Target = "/app/data"
            });
        })
        .WaitFor(redis)
        .WaitFor(filteringService)
        .WaitFor(postgresSql)
        .WithReference(redis)
        .WithReference(scalar)
        .WithReference(filteringService)
        .WithReference(questionDb) // This is the Database for all the Questions that the users can ask.
        .WithReference(nikoDexBackupDb) // The NikoDex Backup Recovery Service's Database.
        .WithReference(approvalDb) // The Approval (Minecraft Server Approval Service)'s Database.
        .WithReference(artPostingDb) // The Arts database.
        .WithEnvironment("ADMIN_USERNAME", adminUsername)
        .WithEnvironment("ADMIN_PASSWORD", adminPassword)
        .WithHttpEndpoint(name: "api");

// The API reference provided by Scalar.
scalar.WithApiReference(backend);

// Add the front-end API to the stack.
// This connects with the main backend (port 5540) and the minecraft backend. (port 7000)
var frontend = builder
    .AddViteApp("fireyfrontend", "../thefirey33.frontend")
    .WithNpm()
    .PublishAsNodeServer("build/index.js", "./build")
    .WithHttpEndpoint(5000)
    .WithExternalHttpEndpoints()
    .WithReference(backend.GetEndpoint("api"))
    .WithReference(filteringService.GetEndpoint("http"))
    .WaitFor(backend);

// The ORIGIN should not be specified if the Application is not in production mode.
if (builder.Environment.IsProduction())
    frontend.WithEnvironment("ORIGIN", "https://thefirey33.net");

// Reference the front-end for the CORS policy.
backend
    .WithReference(frontend.GetEndpoint("http"));

builder.Build().Run();