import xyz.jpenilla.resourcefactory.bukkit.BukkitPluginYaml

plugins {
    id("java-library")
    id("xyz.jpenilla.run-paper") version "3.0.2"
    id("com.gradleup.shadow") version "9.6.1"
    id("io.papermc.paperweight.userdev") version "2.0.0-beta.21"
    id("xyz.jpenilla.resource-factory-bukkit-convention") version "1.3.1"
}


repositories {
    mavenCentral()
    maven("https://repo.papermc.io/repository/maven-public/")
}

dependencies {
    paperweight.paperDevBundle("26.2.build.+")
    implementation("io.javalin:javalin:7.2.2")
    implementation("com.fasterxml.jackson.core:jackson-databind:2.22.1")
    implementation("org.slf4j:slf4j-simple:2.0.18")
}

java {
    toolchain.languageVersion = JavaLanguageVersion.of(25)
}

tasks {

    shadowJar {
        relocate("io.javalin:javalin:7.2.2", "shadow.io.javalin")
        relocate("org.slf4j:slf4j-simple:2.0.18", "shadow.io.slf4j")
        relocate("com.fasterxml.jackson.core:jackson-databind:2.22.1", "shadow.io.jackson")
    }

    runServer {
        // Configure the Minecraft version for our task.
        // This is the only required configuration besides applying the plugin.
        // Your plugin's jar (or shadowJar if present) will be used automatically.
        jvmArgs("-Xms2G", "-Xmx2G", "-Dcom.mojang.eula.agree=true")
    }

    processResources {
        val props = mapOf("version" to version, "description" to project.description)
        filesMatching("plugin.yml") {
            expand(props)
        }
    }
}

bukkitPluginYaml {
    main = "net.thefirey33.thefirey33Fireserver.Thefirey33Fireserver"
    load = BukkitPluginYaml.PluginLoadOrder.STARTUP
    authors.add("Thefirey33")
    apiVersion = "26.1.2"
}