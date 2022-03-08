using Domain;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Reddit.MetaData;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.Caching;
using System.Text;

namespace Data
{
    public class Authorization : IAuthorization
    {
        private readonly ILogger _logger;
        private readonly HttpClient _authClient;

        private static readonly string _userClientID = Environment.GetEnvironmentVariable("RedditClientID");
        private static readonly string _userClientSecret = Environment.GetEnvironmentVariable("RedditClientSecret");
        private static readonly string _refreshToken = Environment.GetEnvironmentVariable("RedditRefreshToken");

        public Authorization(ILogger log)
        {
            _logger = log;
            _authClient = new HttpClient();
        }

        public Token GetToken()
        {
            Token returnToken;

            TokenLog("Start");
            TokenLog("Checking MemoryCache");

            if (MemoryCache.Default.Get("MemToken") == null)
            {
                TokenLog("Not in Cache");
                string bothClientInfo = Convert.ToBase64String(Encoding.UTF8.GetBytes(_userClientID + ":" + _userClientSecret));

                _authClient.DefaultRequestHeaders.Add("Authorization", $"Basic {bothClientInfo}");

                Dictionary<string, string> bodyArgs = new Dictionary<string, string>
                {
                    { "grant_type", "refresh_token"},
                    { "refresh_token", _refreshToken}
                };

                FormUrlEncodedContent tokenRequestBody = new FormUrlEncodedContent(bodyArgs);

                TokenLog("Send");

                HttpResponseMessage refreshResponse = _authClient.PostAsync("https://www.reddit.com/api/v1/access_token", tokenRequestBody).GetAwaiter().GetResult();
                string returnMessage = refreshResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                TokenLog("Recieved");

                returnToken = JsonConvert.DeserializeObject<Token>(returnMessage);

                MemoryCache.Default.Set("MemToken", returnToken, DateTimeOffset.Now.AddMinutes(59));

                TokenLog("Added Token to Memory");
            }
            else
            {
                TokenLog("Retrieve from Cache");
                returnToken = (Token)MemoryCache.Default.Get("MemToken");
            }

            TokenLog("Return");
            return returnToken;
        }

        private void TokenLog(string logString)
        {
            _logger.LogInformation($"GetToken - {logString} - {DateTime.Now}");
        }
    }
}
