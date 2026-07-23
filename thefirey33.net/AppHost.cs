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
        .WithRedisCommander();

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
    builder.AddProject<Projects.thefirey33_backend>("fireybackend")
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

const int gradleServerApiPort = 7000;

// This is the Minecraft server that runs in a docker container.
// It exposes the default Minecraft Server port, and automatically starts.
var gradleMinecraftServer = builder
    .AddDockerfile("fireyminecraftserver", "../thefirey33-fireserver")
    .WithHttpEndpoint(25565, 25565, isProxied: false)
    .WithHttpEndpoint(gradleServerApiPort, gradleServerApiPort, "serverapi", isProxied: false)
    .WithEnvironment("SPRINGBOOT_PORT", gradleServerApiPort.ToString)
    .WithReference(backend.GetEndpoint("http"))
    .WithLifetime(ContainerLifetime.Persistent)
    .WithExternalHttpEndpoints()
    .WithVolume("fireyminecraftserver-volume","/server")
    .WithDockerfileBuilder("../thefirey33-fireserver", context =>
    {
        var javaSdkStage = context.Builder.From("openjdk:25-rc-jdk");
        
        // Copy the server to the specified directory.
        javaSdkStage.WorkDir("/server");
        javaSdkStage.Copy(".", ".");
        
        // Run the server with the entrypoint command.
        javaSdkStage.Run("chmod +x ./gradlew");
        javaSdkStage.Expose(25565);
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