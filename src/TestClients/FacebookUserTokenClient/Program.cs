﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace FacebookUserTokenClient
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            MainAsync().Wait();
        }
        public static async Task MainAsync()
        {
            var baseUrl = "http://localhost:5000";

            Console.WriteLine("Enter the Facebook User Token (see https://developers.facebook.com/tools/accesstoken):");
            string fbToken = Console.ReadLine();

            var client = new HttpClient
            {
                BaseAddress = new Uri(baseUrl)
            };


            var accessTokenResult = await client.GetAccessTokenAsync(fbToken);
            if (accessTokenResult.Error != null)
            {
                Console.WriteLine($"Error: {accessTokenResult.Error}");
                return;
            }
            Console.WriteLine($"Access token: {accessTokenResult.AccessToken}");
        }

        public static async Task<AccessTokenResult> GetAccessTokenAsync(this HttpClient client, string facebookUserAccessToken)
        {
            const string client_id = "devclient";
            const string client_secret = "devsecret";
            const string scope = "openid profile nether-all";

            var requestBody = new FormUrlEncodedContent(
                   new Dictionary<string, string>
                   {
                        { "token", facebookUserAccessToken },
                        { "grant_type", "fb-usertoken" },
                        { "client_id",  client_id },
                        { "client_secret", client_secret },
                        { "scope", scope }
                   }
               );
            var response = await client.PostAsync("/connect/token", requestBody);
            dynamic responseBody = await response.Content.ReadAsAsync<dynamic>();

            if (responseBody.error != null)
            {
                return new AccessTokenResult { Error = responseBody.Error};
            }

            var access_token = (string)responseBody.access_token;
            return new AccessTokenResult
            {
                AccessToken = access_token
            };
        }

        public class AccessTokenResult
        {
            public string Error { get; set; }
            public string  AccessToken { get; set; }
        }
    }
}
