using Microsoft.AspNetCore.Authorization;
using OpenIddict.Validation.AspNetCore;

namespace Presentation;

public class OpenIdDictAuthorize : AuthorizeAttribute
{
    public OpenIdDictAuthorize() => 
        AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
}