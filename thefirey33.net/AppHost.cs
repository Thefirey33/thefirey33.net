using Projects;
using Scalar.Aspire;

var builder = DistributedApplication.CreateBuilder(args);

#pragma warning disable ASPIREJAVASCRIPT001
#pragma warning disable ASPIREDOCKERFILEBUILDER001

var compose =
    builder.AddDockerComposeEnvironment("compose");


var adminUsername = builder.AddParameter("admin-username");
var adminPassword = builder.AddParameter("admin-password");

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
        .WithLifetime(ContainerLifetime.Persistent)
        .WithPgAdmin();

// This is where the backups of the NikoDex are stored.
// Every month, the backend will request to the NikoDex and will store a backup of the Dex.
// With this, the Dex, if in case of emergency, will have a backup to go to.
var nikoDexBackupDb = postgresSql.AddDatabase("nikodexdb");

// This is where all the arts that were made for Thefirey33, or by me will be uploaded.
var artPostingDb = postgresSql.AddDatabase("artdb");

// This will be used for one game in the website, called "Kasane Teto Staring Simulator"
var highScoreDb = postgresSql.AddDatabase("scoredb");

var scalar = builder.AddScalarApiReference();

// This is the Minecraft Server.
// Managed by the FireServer Minecraft Plugin.
var backend =
    builder.AddProject<thefirey33_backend>("fireybackend")
        .WaitFor(redis)
        .WaitFor(postgresSql)
        .WithReference(redis)
        .WithReference(nikoDexBackupDb)
        .WithReference(artPostingDb)
        .WithReference(highScoreDb)
        .WithEnvironment("ADMIN_USERNAME", adminUsername)
        .WithEnvironment("ADMIN_PASSWORD", adminPassword)
        .WithHttpEndpoint(5540, 5540, isProxied: false, name: "api");

scalar.WithApiReference(backend);


// This is the Minecraft server that runs in a docker container.
// It exposes the default Minecraft Server port, and automatically starts.
var gradleMinecraftServer = builder
    .AddDockerfile("fireyminecraftserver", "../thefirey33-fireserver")
    .WithHttpEndpoint(25565, 25565, isProxied: false)
    .WithExternalHttpEndpoints()
    .WithLifetime(ContainerLifetime.Persistent)
    .WithContainerRuntimeArgs("-m", "1g", "--memory-swap", "8g")
    .WithVolume("fireyminecraftserver-volume", "/server")
    .WithDockerfileBuilder("../thefirey33-fireserver", context =>
    {
        var javaSdkStage = context.Builder.From("eclipse-temurin:17-jdk-alpine");
        // Copy the server to the specified directory.
        javaSdkStage.WorkDir("/server");

        // Stop caching for the copying process.
        javaSdkStage.Copy(".", ".");

        // Continue caching after it's done.
        // Run the server with the entrypoint command.
        javaSdkStage.Run("chmod +x ./gradlew");
        javaSdkStage.Expose(25565);
        // Build and run the server.

        javaSdkStage.Entrypoint(["./gradlew", "runServer"]);
        return Task.CompletedTask;
    });

// Add the front end API to the stack.
var frontend = builder
    .AddViteApp("fireyfrontend", "../thefirey33-frontend")
    .PublishAsNodeServer("build/index.js", "build")
    .WithHttpEndpoint(5000, 5000, isProxied: false)
    .PublishAsStaticWebsite()
    .WithExternalHttpEndpoints()
    .WaitFor(backend);

// This is the backend for the website.
// Showing the status of the Minecraft Server and keeping backups of the NikoDex.
backend
    .WithReference(frontend.GetEndpoint("http"))
    .WithReference(gradleMinecraftServer.GetEndpoint("http"));

// Reference the frontend with the backend.
frontend
    .WithReference(backend.GetEndpoint("api"));

builder.Build().Run();