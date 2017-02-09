using System.Collections.Generic;
using IdentityServer3.Core;
using IdentityServer3.Core.Models;

namespace IdentityServer
{
    public static class Clients
    {
        public static IEnumerable<Client> Get()
        {
            return new List<Client> {
            new Client {
                ClientId = "implicitclient",
                ClientName = "Example Implicit Client",
                Enabled = true,
                Flow = Flows.Implicit,
                RequireConsent = true,
                AllowRememberConsent = true,
                RedirectUris =
                  new List<string> {"https://localhost:44300/account/signInCallback"},
                PostLogoutRedirectUris =
                  new List<string> {"https://localhost:44300/"},
                AllowedScopes = new List<string> {
                    Constants.StandardScopes.OpenId,
                    Constants.StandardScopes.Profile,
                    Constants.StandardScopes.Email
                },
                AccessTokenType = AccessTokenType.Jwt
            }
        };
        }
    }
}