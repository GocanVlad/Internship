﻿using Microsoft.Identity.Client;  // Microsoft Authentication Library (MSAL)

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;

namespace PowerApps.Samples
{
    /// <summary>
    /// Demonstrates Azure authentication and execution of a Dataverse Web API function.
    /// </summary>
    class Program
    {
        static void Main()
        {
            // TODO Specify the Dataverse environment name to connect with.
            string resource = "https://<env-name>.api.<region>.dynamics.com";

            // Azure Active Directory app registration shared by all Power App samples.
            // For your custom apps, you will need to register them with Azure AD yourself.
            // See https://docs.microsoft.com/powerapps/developer/data-platform/walkthrough-register-app-azure-active-directory
            var clientId = "51f81489-12ee-4a9e-aaae-a2591f45987d";
            var redirectUri = "http://localhost"; // Loopback for the interactive login.

            #region Authentication

            var authBuilder = PublicClientApplicationBuilder.Create(clientId)
                             .WithAuthority(AadAuthorityAudience.AzureAdMultipleOrgs)
                             .WithRedirectUri(redirectUri)
                             .Build();
            var scope = resource + "/.default";
            string[] scopes = { scope };

            AuthenticationResult token =
                authBuilder.AcquireTokenInteractive(scopes).ExecuteAsync().Result;
            #endregion Authentication

            #region Client configuration

            var client = new HttpClient
            {
                // See https://docs.microsoft.com/powerapps/developer/data-platform/webapi/compose-http-requests-handle-errors#web-api-url-and-versions
                BaseAddress = new Uri(resource + "/api/data/v9.2/"),
                Timeout = new TimeSpan(0, 2, 0)    // Standard two minute timeout on web service calls.
            };

            // Default headers for each Web API call.
            // See https://docs.microsoft.com/powerapps/developer/data-platform/webapi/compose-http-requests-handle-errors#http-headers
            HttpRequestHeaders headers = client.DefaultRequestHeaders;
            headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
            headers.Add("OData-MaxVersion", "4.0");
            headers.Add("OData-Version", "4.0");
            headers.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            #endregion Client configuration

            #region Web API call

            // Invoke the Web API 'WhoAmI' unbound function.
            // See https://docs.microsoft.com/powerapps/developer/data-platform/webapi/compose-http-requests-handle-errors
            // See https://docs.microsoft.com/powerapps/developer/data-platform/webapi/use-web-api-functions#unbound-functions
            var response = client.GetAsync("WhoAmI").Result;

            if (response.IsSuccessStatusCode)
            {
                // Parse the JSON formatted service response (WhoAmIResponse) to obtain the user ID value.
                // See https://docs.microsoft.com/dynamics365/customer-engagement/web-api/whoamiresponse
                Guid userId = new Guid();

                String jsonContent = response.Content.ReadAsStringAsync().Result;

                using (JsonDocument doc = JsonDocument.Parse(jsonContent))
                {
                    JsonElement root = doc.RootElement;
                    JsonElement userIdElement = root.GetProperty("UserId");
                    userId = userIdElement.GetGuid();
                }

                // Alternate code, but requires that the WhoAmIResponse class be defined (see below).
                //WhoAmIResponse whoAmIresponse = JsonSerializer.Deserialize<WhoAmIResponse>(jsonContent);
                //userId = whoAmIresponse.UserId;

                Console.WriteLine("Your user ID is {0}", userId.ToString());
            }
            else
            {
                Console.WriteLine("Web API call failed");
                Console.WriteLine("Reason: " + response.ReasonPhrase);
            }
            #endregion Web API call

            // Pause program execution by waiting for a key press.
            Console.ReadKey();
        }
    }

    /// <summary>
    /// WhoAmIResponse class definition 
    /// </summary>
    /// <remarks>To be used for JSON deserialization.</remarks>
    /// <see cref="https://docs.microsoft.com/dynamics365/customer-engagement/web-api/whoamiresponse"/>
    public class WhoAmIResponse
    {
        public Guid BusinessUnitId { get; set; }
        public Guid UserId { get; set; }
        public Guid OrganizationId { get; set; }
    }
}