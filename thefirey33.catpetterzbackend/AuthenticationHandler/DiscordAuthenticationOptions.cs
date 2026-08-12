using Microsoft.AspNetCore.Authentication;

namespace thefirey33.catpetterzBackend.AuthenticationHandler;

public class DiscordAuthenticationOptions : AuthenticationSchemeOptions
{
    public const string DefaultScheme = "DiscordAuthenticationScheme";
}