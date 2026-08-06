using System.Net.Sockets;
using Aspire.Hosting.Docker.Resources.ComposeNodes;
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


const string wireguardNetworkName = "wireguard-network";

// Configure the wireguard network, so the Discord API can be called without SSL handshake errors.
compose.ConfigureComposeFile(options =>
{
    options.AddNetwork(new Network
    {
        Name = wireguardNetworkName,
        Driver = "bridge"
    });
});

var wireguardContainer = builder.AddContainer("fireywireguard", "metaligh/amneziawg")
    .WithEndpoint(51820, 51820, protocol: ProtocolType.Udp)
    .PublishAsDockerComposeService((_, service) =>
    {
        service.CapAdd = ["NET_ADMIN", "SYS_MODULE"];
        service.Devices = ["/dev/net/tun"];
        service.Restart = "unless-stopped";
        service.Networks = [wireguardNetworkName];
    });

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
    .PublishAsDockerComposeService((_, service) => { service.Networks = [wireguardNetworkName]; })
    .WithEnvironment("CLIENT_ID", builder.AddParameter("bot-client-id", true))
    .WithEnvironment("CLIENT_SECRET", builder.AddParameter("bot-client-secret", true))
    .WithEnvironment("REDIRECT_URI", builder.AddParameter("bot-redirect-uri"))
    .WithEnvironment("BOT_TOKEN", builder.AddParameter("bot-token", true))
    .WithHttpHealthCheck("/health")
    .WithHttpEndpoint(env: "PORT");

// This is the Minecraft Server.
// Managed by the FireServer Minecraft Plugin.
var backend =
    builder.AddProject<thefirey33_backend>("fireybackend")
        .PublishAsDockerComposeService((_, service) =>
        {
            service.Name = "fireybackend";
            service.Networks.Add(wireguardNetworkName);
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

// The endpoint of the Minecraft Server.
const int minecraftServerApiEndpoint = 7000;

// This is the Minecraft server that runs in a docker container.
// It exposes the default Minecraft Server port, and automatically starts.
var gradleMinecraftServer = builder
    .AddDockerfile("fireyminecraftserver", "../thefirey33.fireserver")
    .WithEndpoint(25565, 25565, isExternal: true)
    .WithHttpEndpoint(minecraftServerApiEndpoint, minecraftServerApiEndpoint, "api", "SERVER_ENDPOINT")
    .WithEnvironment("TRUSTED_OPERATOR_UUID", trustedOperatorUuid)
    .WithEnvironment("ADMIN_USERNAME", adminUsername)
    .WithEnvironment("ADMIN_PASSWORD", adminPassword)
    .WithReference(backend.GetEndpoint("api"))
    .WithPersistentLifetime()
    .WithVolume("fireservervolume", "/data")
    .WithDockerfileBuilder("../thefirey33.fireserver", context =>
    {
        var fireServerPluginStage = context.Builder.From("eclipse-temurin:25-jdk-alpine", "builderfireserver");
        fireServerPluginStage.WorkDir("/compile");
        fireServerPluginStage.Copy(".", ".");
        fireServerPluginStage.Run("chmod +x ./gradlew");
        fireServerPluginStage.Run("--mount=type=cache,target=/root/.gradle ./gradlew build --no-daemon");

        var runnerStage = context.Builder.From("itzg/minecraft-server:java25-jdk", "runner");
        runnerStage.Run("rm -rf ./plugins");
        runnerStage.Env("VERSION", "26.2");
        runnerStage.Env("MEMORY", "8G");
        runnerStage.Env("EULA", "TRUE"); // Accept the Minecraft EULA.
        runnerStage.Env("TYPE", "PAPER");
        runnerStage.Env("USES_PLUGINS", "true");
        runnerStage.CopyFrom("builderfireserver", "/compile/build/libs/*.jar", "./plugins/");
        runnerStage.Expose(25565);

        return Task.CompletedTask;
    });

// Add the front-end API to the stack.
// This connects with the main backend (port 5540) and the minecraft backend. (port 7000)
var frontend = builder
    .AddViteApp("fireyfrontend", "../thefirey33.frontend")
    .WithNpm()
    .PublishAsNodeServer("build/index.js", "./build")
    .WithHttpEndpoint(5000)
    .WithExternalHttpEndpoints()
    .PublishAsDockerComposeService((resource, service) => { service.Networks.Add(wireguardNetworkName); })
    .WithReference(backend.GetEndpoint("api"))
    .WithReference(filteringService.GetEndpoint("http"))
    .WithReference(gradleMinecraftServer.GetEndpoint("api"))
    .WaitFor(backend);

// The ORIGIN should not be specified if the Application is not in production mode.
if (builder.Environment.IsProduction())
    frontend.WithEnvironment("ORIGIN", "https://thefirey33.net");

// Reference the front-end for the CORS policy.
backend
    .WithReference(frontend.GetEndpoint("http"))
    .WithReference(gradleMinecraftServer.GetEndpoint("api"));

builder.Build().Run();