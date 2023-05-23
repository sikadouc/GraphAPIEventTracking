// See https://aka.ms/new-console-template for more information
using GetCalendarEvents.Helpers;
using Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph.Users.Item.Calendar.Events.Delta;

public class Program
{
    private static string? skipToken = null;
    private static string? deltaToken = null;

    public static void Main(string[] args)
    {
        var userId = "userId";
        var startDateTime = DateTime.UtcNow;
        var endDateTime = DateTime.UtcNow.AddDays(7);

        var config = LoadAppSettings();
        if (config == null)
        {
            Console.WriteLine("Invalid appsettings.json file.");
            return;
        }
        var authenticationHelper = new AuthenticationHelper(config);
        var graphServiceClient = GraphHelper.GetAuthenticatedGraphServiceClient(authenticationHelper.GetClientSecretCredential(),
             new string[] { config["scopes"] });
        var eventsHelper = new EventsHelper();
        //first Call to get all pages

        var response = eventsHelper.GetEvents(graphServiceClient, userId, startDateTime, endDateTime, 5, skipToken);
        var result = response.Result;
        skipToken = UrlHelper.GetQueryParam(response.Result.OdataNextLink, "$skiptoken");

        while (!string.IsNullOrEmpty(skipToken))
        {
             response = eventsHelper.GetEvents(graphServiceClient, userId, startDateTime, endDateTime, 5, skipToken);
             result = response.Result;
            skipToken = UrlHelper.GetQueryParam(response.Result.OdataNextLink, "$skiptoken");
            deltaToken = UrlHelper.GetQueryParam(response.Result.OdataNextLink, "$deltatoken");
        }

        // call to get all delta in the future (code to externalise in a separate worker
        while (true && !string.IsNullOrEmpty(deltaToken))
        {
            response = eventsHelper.GetDeltaEvents(graphServiceClient,userId,deltaToken);
       
            deltaToken = UrlHelper.GetQueryParam(response.Result.OdataNextLink, "$deltatoken");
            Thread.Sleep(30000);
        }
    }

    private static IConfigurationRoot? LoadAppSettings()
    {
        try
        {
            var config = new ConfigurationBuilder()
                              .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                              .AddJsonFile("appsettings.json", false, true)
                              .Build();

            if (string.IsNullOrEmpty(config["applicationId"]) ||
                string.IsNullOrEmpty(config["tenantId"]) ||
                string.IsNullOrEmpty(config["secret"]) ||
                string.IsNullOrEmpty(config["scopes"]))
            {
                return null;
            }

            return config;
        }
        catch (System.IO.FileNotFoundException)
        {
            // Log here.
            throw;
        }
    }
}
