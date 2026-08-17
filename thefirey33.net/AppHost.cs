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

var mongoDb
    = builder.AddMongoDB("catpetterzdatabase")
        .WithImageTag("7.0.21")
        .WithDataVolume()
        .WithMongoExpress();

// This is where the backups of the NikoDex are stored.
// Every month, the backend will request to the NikoDex and will store a backup of the Dex.
// With this, the Dex, if in case of emergency, will have a backup to go to.
var nikoDexBackupDb = postgresSql.AddDatabase("nikodexdb");

// This is where all the arts that were made for Thefirey33, or by me will be uploaded.
var artPostingDb = postgresSql.AddDatabase("artdb");

// This is for the Questions that can be asked on the website.
// It will require Discord Authentication.
var questionDb = postgresSql.AddDatabase("questiondb");

// The CatPetterz NoSQL database.
// This is a NoSQL database because we have to keep thousands of cats in storage,
// So all of them can be updated quickly instead of relying on busying PostgreSQL.
var catpetterzDb = mongoDb.AddDatabase("catpetterzdb");

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
        .WithHttpEndpoint(targetPort: 1080, name: "proxy");

    filteringService.WithEnvironment("PROXY", cloudflareWarpService.GetEndpoint("proxy"));
}
else
{
// The backend for the CatPetterz Game.
// Which manages the databases and authentication.
    var catpetterzBackend
        = builder.AddProject<thefirey33_catpetterzbackend>("catpetterzbackend")
            .PublishAsDockerComposeService((_, service) =>
            {
                // Add the database that contains the data of the catpetterz game.
                service.User = "0:0";

                service.AddVolume(new Volume
                {
                    Type = "volume",
                    Name = "catpetterz-volume",
                    Target = "/data"
                });
            })
            .WaitFor(catpetterzDb)
            .WithReference(catpetterzDb)
            .WithEnvironment("REDIRECT_URI", builder.AddParameter("catpatterz-redirect-uri"))
            .WithReference(filteringService)
            .WithReference(scalar)
            .WaitFor(filteringService);

// The Scalar API reference for the CatPetterz API.
    scalar.WithApiReference(catpetterzBackend);

    var catpetterzGame = builder.AddDockerfile("catpetterzgame", "../thefirey33.catpetterz")
        .WithDockerfileBuilder("../thefirey33.catpetterz", context =>
        {
            // Build the game image to the release image.
            var builderImage = context.Builder.From("barichello/godot-ci", "builder");
            builderImage.WorkDir("/compile");
            builderImage.Copy(".", ".");
            builderImage.Run("mkdir ./build");
            builderImage.Run("""godot --headless --export-debug --verbose "Web" ./build/index.html""");

            // This game is hosted with NGINX as it's hoster server.
            // Then it's proxied by YARP.
            var runnerImage = context.Builder.From("nginx", "runner");
            runnerImage.CopyFrom("builder", "/compile/build", "/usr/share/nginx/html");
            runnerImage.Run("rm /etc/nginx/nginx.conf");
            runnerImage.Copy("nginx.conf", "/etc/nginx/nginx.conf");
            runnerImage.Expose(80);
        })
        .WithHttpEndpoint(targetPort: 80);


// The general gateway of the game.
// This is what manages the game's routing.
    builder.AddYarp("catpetterzgateway")
        .WithHttpEndpoint(7000, 7000)
        .WithExternalHttpEndpoints()
        .WithConfiguration(yarp =>
        {
            yarp.AddRoute(catpetterzGame.GetEndpoint("http"));

            var cluster = yarp.AddCluster(catpetterzBackend);
            yarp.AddRoute("/api/{**catch-all}", cluster);
            yarp.AddRoute("/updategateway/{**catch-all}", cluster);
        });
}

// The backend for the entire website.
// This manages all 
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
        .WithReference(artPostingDb) // The Arts database.
        .WithEnvironment("ADMIN_USERNAME", adminUsername)
        .WithEnvironment("ADMIN_PASSWORD", adminPassword)
        .WithHttpEndpoint(name: "api");

// The API reference provided by Scalar.
scalar.WithApiReference(backend);

// This is for the old frontend.
// Something made for the fun of it, the C++ frontend of the website.
var oldFrontend = builder.AddDockerfile("fireyoldfrontend", "../thefirey33.frontendold")
    .WithDockerfileBuilder("../thefirey33.frontendold", context =>
    {
        var runner = context.Builder.From("alpine:3.14");
        runner.WorkDir("/server");
        runner.Run("apk add --no-cache cmake build-base git");
        runner.Copy(".", ".");
        runner.Run("cmake -S . -B build");
        runner.Run("cmake --build build");
        runner.Expose(8080);

        // Run the old frontend's executable for running.
        runner.Entrypoint(["./build/old_web"]);
    })
    .WaitFor(backend)
    .WithReference(backend.GetEndpoint("api"))
    .WithExternalHttpEndpoints()
    .WithHttpEndpoint(8080, 8080, env: "PORT");

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
    frontend.WithEnvironment("ORIGIN", builder.AddParameter("website-origin"));

backend.WithReference(frontend.GetEndpoint("http"));

builder.Build().Run();