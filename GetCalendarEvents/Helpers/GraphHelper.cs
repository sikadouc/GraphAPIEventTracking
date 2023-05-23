using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetCalendarEvents.Helpers
{
    public static class GraphHelper
    {
        public static GraphServiceClient GetAuthenticatedGraphServiceClient(ClientSecretCredential clientSecretCredential, string[] scopes)
        {
            var graphClient = new GraphServiceClient(clientSecretCredential,scopes);
            return graphClient;
        }
    }
}
