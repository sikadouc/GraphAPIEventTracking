using System.Net.Http.Headers;
using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using Microsoft.Graph.Models;

namespace Helpers
{
    public class AuthenticationHelper
    {
        private readonly string? _clientId;
        private readonly string? _clientSecret;
        private readonly string? _tenantId;


        public AuthenticationHelper(IConfigurationRoot config)
        {
            _tenantId = config["tenantId"];
            _clientId = config["applicationId"];
            _clientSecret = config["secret"];
        }

        public ClientSecretCredential GetClientSecretCredential()
        {
            var options = new TokenCredentialOptions
            {
                AuthorityHost = AzureAuthorityHosts.AzurePublicCloud
            };            
            return  new ClientSecretCredential(_tenantId, _clientId, _clientSecret, options);
        }

        public ClientSecretCredential GetClientSecretCredential(IConfigurationRoot config)
        {
            var options = new TokenCredentialOptions
            {
                AuthorityHost = AzureAuthorityHosts.AzurePublicCloud
            };
            return new ClientSecretCredential(config["tenantId"], config["clientId"], config["secret"], options);
        }
    }
}