using Microsoft.AspNetCore.Authentication;

namespace CoreVisionFoundation.Security
{
    public class CodeVisionAuthenticationSchemeOptions : AuthenticationSchemeOptions
    {
        public string JwtTokenSigningKey { get; set; }
    }
}
