using System;
using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest;
using Newtonsoft.Json;

namespace PdfSign.Azure
{
    public class CredentialsService
    {
        private static string ApplicationId = "";
        private static string ApplicationObjectId = "";
        private static string ServicePrincipalId = "";

        private string TenantId = "";
        private string ClientId = ApplicationId;
        private string Password = "";

        public async Task<ServiceClientCredentials> GetCredentials()
        {
            var authenticationContext = new AuthenticationContext($"https://login.windows.net/{TenantId}");

            AuthenticationResult result = null;

            result = await authenticationContext.AcquireTokenAsync(
                "https://management.azure.com/",
                new ClientCredential(ClientId, Password));

            if (result == null)
            {
                throw new InvalidOperationException("Failed to obtain the JWT token");
            }

            return new TokenCredentials(result.AccessToken);
        }
    }
}