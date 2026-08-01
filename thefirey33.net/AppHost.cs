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
        .WithDataVolume(isReadOnly: false)
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

var scalar = builder.AddScalarApiReference();

// This is the Minecraft Server.
// Managed by the FireServer Minecraft Plugin.
var backend =
    builder.AddProject<thefirey33_backend>("fireybackend")
        .PublishAsDockerComposeService((resource, service) =>
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
        .WaitFor(postgresSql)
        .WithReference(redis)
        .WithReference(nikoDexBackupDb)
        .WithReference(approvalDb)
        .WithReference(artPostingDb)
        .WithEnvironment("ADMIN_USERNAME", adminUsername)
        .WithEnvironment("ADMIN_PASSWORD", adminPassword)
        .WithHttpEndpoint(5540, 5540, isProxied: false, name: "api");

scalar.WithApiReference(backend);

const int minecraftServerApiEndpoint = 7000;

// This is the Minecraft server that runs in a docker container.
// It exposes the default Minecraft Server port, and automatically starts.
var gradleMinecraftServer = builder
    .AddDockerfile("fireyminecraftserver", "../thefirey33-fireserver")
    .WithEndpoint(25565, 25565, isProxied: false, isExternal: true)
    .WithHttpEndpoint(minecraftServerApiEndpoint, minecraftServerApiEndpoint, isProxied: false, name: "api")
    .WithEnvironment("SERVER_ENDPOINT", minecraftServerApiEndpoint.ToString)
    .WithEnvironment("TRUSTED_OPERATOR_UUID", trustedOperatorUuid)
    .WithEnvironment("ADMIN_USERNAME", adminUsername)
    .WithEnvironment("ADMIN_PASSWORD", adminPassword)
    .WithReference(backend.GetEndpoint("api"))
    .WithPersistentLifetime()
    .WithVolume("fireservervolume", "/data")
    .WithDockerfileBuilder("../thefirey33-fireserver", context =>
    {
        var fireServerPluginStage = context.Builder.From("eclipse-temurin:25-jdk-alpine", "builderfireserver");
        fireServerPluginStage.WorkDir("/compile");
        fireServerPluginStage.Copy(".", ".");
        fireServerPluginStage.Run("chmod +x ./gradlew");
        fireServerPluginStage.Run("--mount=type=cache,target=/root/.gradle ./gradlew build --no-daemon");

        var runnerStage = context.Builder.From("itzg/minecraft-server:java25-jdk", "runner");
        if (!builder.Environment.IsDevelopment()) runnerStage.Env("MEMORYSIZE", "6G");
        runnerStage.Run("rm -rf ./plugins");
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
    .AddViteApp("fireyfrontend", "../thefirey33-frontend")
    .WithNpm()
    .PublishAsNodeServer("build/index.js", "./build")
    .WithHttpEndpoint(5000, 5000, isProxied: false)
    .WithExternalHttpEndpoints()
    .WithReference(backend.GetEndpoint("api"))
    .WithEnvironment("ORIGIN", builder.AddParameter("origin", true))
    .WithReference(gradleMinecraftServer.GetEndpoint("api"))
    .WaitFor(backend);

// Reference the front-end for the CORS policy.
backend
    .WithReference(frontend.GetEndpoint("http"))
    .WithReference(gradleMinecraftServer.GetEndpoint("api"));

builder.Build().Run();