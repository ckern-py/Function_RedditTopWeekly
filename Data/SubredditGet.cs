using Domain;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Reddit.MetaData;
using System;
using System.Net.Http;

namespace Data
{
    public class SubredditGet : ISubredditGet
    {
        private readonly ILogger _logger;
        private readonly IAuthorization _authorization;

        private readonly HttpClient _subredditInfoClient;
        private Token _currToken;

        public SubredditGet(ILogger log)
        {
            _logger = log;
            _subredditInfoClient = new HttpClient();
            _authorization = new Authorization(log);
        }

        public Rootobject GetRandomSubreddit()
        {
            SubredditInfoLog("GetRandomSub", "Start");
            _currToken = _authorization.GetToken();

            SubredditInfoLog("GetRandomSub", "Got Token");
            _subredditInfoClient.DefaultRequestHeaders.Clear();
            _subredditInfoClient.DefaultRequestHeaders.Add("authorization", $"Bearer {_currToken.Access_token}");
            _subredditInfoClient.DefaultRequestHeaders.Add("user-agent", "WeeklyBotApp/1.0 by me");

            SubredditInfoLog("GetRandomSub", "Send");
            HttpResponseMessage responseToRandom = _subredditInfoClient.GetAsync("https://oauth.reddit.com/r/random?raw_json=1").Result;

            string randomSub = responseToRandom.RequestMessage.RequestUri.Segments[2];

            SubredditInfoLog("GetRandomSub", "Received");

            Rootobject randomSubContent = GetSubWeeklyTop(randomSub);

            SubredditInfoLog("GetRandomSub", "Return");
            return randomSubContent;
        }

        private Rootobject GetSubWeeklyTop(string specificSubreddit)
        {
            SubredditInfoLog("GetRandomSub", "Start");

            if (specificSubreddit.Trim().EndsWith('/'))
            {
                specificSubreddit = specificSubreddit.Trim().TrimEnd('/');
            }

            SubredditInfoLog("GetRandomSub", "Send");
            HttpResponseMessage responseToRandom = _subredditInfoClient.GetAsync($"https://oauth.reddit.com/r/{specificSubreddit}/top?t=week&limit=1&raw_json=1").Result;

            string subredditNewContent = responseToRandom.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            SubredditInfoLog("GetRandomSub", "Received");

            if (!responseToRandom.IsSuccessStatusCode)
            {
                throw new Exception("Retrieving random subreddit was not successful.");
            }

            SubredditInfoLog("GetRandomSub", "Return");
            return JsonConvert.DeserializeObject<Rootobject>(subredditNewContent);
        }

        private void SubredditInfoLog(string method, string logString)
        {
            _logger.LogInformation($"{method} - {logString} - {DateTime.Now}");
        }
    }
}
