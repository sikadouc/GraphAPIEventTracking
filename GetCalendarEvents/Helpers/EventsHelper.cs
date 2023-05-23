using Microsoft.Graph;
using Azure;
using System.Xml.Linq;
using Microsoft.Kiota.Abstractions;
using Microsoft.Graph.Models;
using Microsoft.Graph.Users.Item.CalendarView.Delta;

public class EventsHelper
{


    public async Task<DeltaResponse?> GetEvents(GraphServiceClient graphClient, string userId, DateTime startDate, DateTime endDate, int maxEventsPerPage, string? skipToken)
    {

        
        string isoStartDate = startDate.ToString("yyyy-MM-ddTHH:mm:ssZ");
        string isoEndDate = endDate.ToString("yyyy-MM-ddTHH:mm:ssZ");
        RequestInformation? requestInformation = null;

        if (string.IsNullOrEmpty(skipToken))
        {
            requestInformation = graphClient.Users[userId].CalendarView.Delta.ToGetRequestInformation(((requestConfiguration) =>
        {
            //requestConfiguration.QueryParameters.Filter = $"start/dateTime gt '{startDate}' and start/dateTime lt '{endDate}'";
            requestConfiguration.Headers.Add("Prefer", $"odata.maxpagesize={maxEventsPerPage}");
        }));
            requestInformation.UrlTemplate = requestInformation.UrlTemplate.Insert(requestInformation.UrlTemplate.Length - 1, ",%24Startdatetime,%24Enddatetime");
            requestInformation.QueryParameters.Add("Startdatetime ", isoStartDate);
            requestInformation.QueryParameters.Add("Enddatetime ", isoEndDate);
        }
        else
        {
            requestInformation = graphClient.Users[userId].CalendarView.Delta.ToGetRequestInformation(((requestConfiguration) =>
            {
                requestConfiguration.Headers.Add("Prefer", $"odata.maxpagesize={maxEventsPerPage}");
            }));
            requestInformation.UrlTemplate = requestInformation.UrlTemplate.Insert(requestInformation.UrlTemplate.Length - 1, ",%24skiptoken,%24deltatoken,changeType");
            requestInformation.QueryParameters.Add("%24skiptoken", skipToken);
        }

        var result = await graphClient.RequestAdapter.SendAsync<DeltaResponse>(requestInformation, DeltaResponse.CreateFromDiscriminatorValue);

        return result;
    }

    public async Task<DeltaResponse?> GetDeltaEvents(GraphServiceClient graphClient, string userId, string? deltaToken)
    {

        RequestInformation? requestInformation = null;

        requestInformation = graphClient.Users[userId].CalendarView.Delta.ToGetRequestInformation();
        requestInformation.UrlTemplate = requestInformation.UrlTemplate.Insert(requestInformation.UrlTemplate.Length - 1, ",%24skiptoken,%24deltatoken,changeType");
        requestInformation.QueryParameters.Add("%24deltatoken", deltaToken);
        requestInformation.QueryParameters.Add("changeType", "created");

        var result = await graphClient.RequestAdapter.SendAsync<DeltaResponse>(requestInformation, DeltaResponse.CreateFromDiscriminatorValue);

        return result;
    }

}