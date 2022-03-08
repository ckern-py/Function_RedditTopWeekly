using Domain;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Reddit.MetaData;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Data
{
    public class SubredditDo : ISubredditDo
    {
        private readonly ILogger _logger;
        private readonly IAuthorization _authorization;

        private readonly HttpClient _subredditInfoClient;
        private Token _currToken;


        public SubredditDo(ILogger log)
        {
            _logger = log;
            _subredditInfoClient = new HttpClient();
            _authorization = new Authorization(log);
        }

        public Post_Rootobject SubredditSubmit(Rootobject postFromInfo)
        {
            string postTitle = CreatePostTitle(postFromInfo);

            SubredditInfoLog("SubredditSubmit", "Start");
            _currToken = _authorization.GetToken();

            SubredditInfoLog("SubredditSubmit", "Got Token");
            _subredditInfoClient.DefaultRequestHeaders.Clear();
            _subredditInfoClient.DefaultRequestHeaders.Add("authorization", $"Bearer {_currToken.Access_token}");
            _subredditInfoClient.DefaultRequestHeaders.Add("user-agent", "WeeklyBotApp/1.0 by me");

            string subredditName = Environment.GetEnvironmentVariable("SubredditName");

            List<KeyValuePair<string, string>> content = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("ad", "false"),
                new KeyValuePair<string, string>("api_type", "json")
            };
            if (postFromInfo.Data.Children[0].Data.Is_self)
            {
                content.Add(new KeyValuePair<string, string>("kind", "self"));
                content.Add(new KeyValuePair<string, string>("text", postFromInfo.Data.Children[0].Data.Selftext));
            }
            else
            {
                content.Add(new KeyValuePair<string, string>("kind", "link"));
                content.Add(new KeyValuePair<string, string>("url", postFromInfo.Data.Children[0].Data.Url));
            }
            content.Add(new KeyValuePair<string, string>("resubmit", "true"));
            content.Add(new KeyValuePair<string, string>("nsfw", "false"));
            content.Add(new KeyValuePair<string, string>("sendreplies", "false"));
            content.Add(new KeyValuePair<string, string>("spoiler", postFromInfo.Data.Children[0].Data.Spoiler.ToString()));
            content.Add(new KeyValuePair<string, string>("sr", subredditName));
            content.Add(new KeyValuePair<string, string>("title", postTitle));

            FormUrlEncodedContent formUrlEncodedContent = new FormUrlEncodedContent(content);

            SubredditInfoLog("SubredditSubmit", "Send");
            HttpResponseMessage postResponse = _subredditInfoClient.PostAsync("https://oauth.reddit.com/api/submit", formUrlEncodedContent).Result;

            if (!postResponse.IsSuccessStatusCode)
            {
                throw new Exception("Posting to subreddit was not successful.");
            }

            SubredditInfoLog("SubredditSubmit", "Received");
            string postResponseContent = postResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            SubredditInfoLog("SubredditSubmit", "Return");
            return JsonConvert.DeserializeObject<Post_Rootobject>(postResponseContent);
        }

        public void SubredditPostComment(Rootobject mainRoot, Post_Rootobject postRoot)
        {
            string markDownComment = CreateComment(mainRoot);

            SubredditInfoLog("SubredditPostComment", "Start");
            _currToken = _authorization.GetToken();

            SubredditInfoLog("SubredditPostComment", "Got Token");
            _subredditInfoClient.DefaultRequestHeaders.Clear();
            _subredditInfoClient.DefaultRequestHeaders.Add("authorization", $"Bearer {_currToken.Access_token}");
            _subredditInfoClient.DefaultRequestHeaders.Add("user-agent", "WeeklyBotApp/1.0 by me");

            List<KeyValuePair<string, string>> comment = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("api_type", "json"),
                new KeyValuePair<string, string>("text", markDownComment),
                new KeyValuePair<string, string>("thing_id", postRoot.Json.Data.Name),
                new KeyValuePair<string, string>("return_rtjson", "false")
            };

            FormUrlEncodedContent formUrlEncodedContentComment = new FormUrlEncodedContent(comment);

            SubredditInfoLog("SubredditPostComment", "Send");
            HttpResponseMessage commentResponse = _subredditInfoClient.PostAsync("https://oauth.reddit.com/api/comment", formUrlEncodedContentComment).Result;

            SubredditInfoLog("SubredditPostComment", "Received");
            if (!commentResponse.IsSuccessStatusCode)
            {
                throw new Exception("Posting comment was not successful.");
            }

            SubredditInfoLog("SubredditPostComment", "Return");
        }

        private string CreatePostTitle(Rootobject createMaterial)
        {
            SubredditInfoLog("CreatePostTitle", "Start");
            string postTitle;
            int availableSpace = 265;

            availableSpace -= createMaterial.Data.Children[0].Data.Subreddit.Length;

            if (createMaterial.Data.Children[0].Data.Title.Length > availableSpace)
            {
                postTitle = createMaterial.Data.Children[0].Data.Title.Substring(0, availableSpace - 3) + "...";
            }
            else
            {
                postTitle = createMaterial.Data.Children[0].Data.Title;
            }

            SubredditInfoLog("CreatePostTitle", "Return");
            return $"Top post from r/{createMaterial.Data.Children[0].Data.Subreddit} | {postTitle} | {DateTime.Now:MMM dd yyyy}";
        }

        private string CreateComment(Rootobject commentMaterial)
        {
            SubredditInfoLog("CreateComment", "Start");

            string fullComment = string.Empty;
            string newNewLine = "\n\n";
            DateTimeOffset postedDate = DateTimeOffset.FromUnixTimeSeconds((long)commentMaterial.Data.Children[0].Data.Created_utc);

            fullComment += "## Stats for the original post, as of right now." + newNewLine;
            fullComment += $"---" + newNewLine;
            fullComment += $"**Subreddit**: {commentMaterial.Data.Children[0].Data.Subreddit_name_prefixed}" + newNewLine;
            fullComment += $"**Title**: {commentMaterial.Data.Children[0].Data.Title}" + newNewLine;
            fullComment += $"**Total Awards**: {commentMaterial.Data.Children[0].Data.Total_awards_received}" + newNewLine;
            fullComment += $"**Upvotes**: {commentMaterial.Data.Children[0].Data.Ups}" + newNewLine;
            fullComment += $"**Comments**: {commentMaterial.Data.Children[0].Data.Num_comments}" + newNewLine;
            fullComment += $"**Created**: {postedDate.DateTime} UTC" + newNewLine;
            fullComment += $"**Link to Thread**: https://www.reddit.com{commentMaterial.Data.Children[0].Data.Permalink}" + newNewLine;
            fullComment += $"---" + newNewLine;
            fullComment += "^(This comment was created by a bot.)"; // Fancy links and stuff??

            SubredditInfoLog("CreateComment", "Return");
            return fullComment;
        }

        private void SubredditInfoLog(string method, string logString)
        {
            _logger.LogInformation($"{method} - {logString} - {DateTime.Now}");
        }
    }
}
