using Microsoft.AspNetCore.Authentication;

namespace CoreVisionFoundation.Security
{
    public class CoreVisionAuthenticationSchemeOptions : AuthenticationSchemeOptions
    {
        public string JwtTokenSigningKey { get; set; }
    }
}
