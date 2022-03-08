using Domain;
using Microsoft.Extensions.Logging;
using Reddit.MetaData;
using System;

namespace Data
{
    public class SubredditProcess : ISubredditProcess
    {
        private readonly ILogger _logger;
        private readonly ISubredditGet _subredditGet;
        private readonly ISubredditDo _subredditDo;

        public SubredditProcess(ILogger log)
        {
            _logger = log;
            _subredditGet = new SubredditGet(log);
            _subredditDo = new SubredditDo(log);
        }

        public void Process()
        {
            SubredditProcessLog("Getting random sub");
            Rootobject ranSubRootObj = _subredditGet.GetRandomSubreddit();

            SubredditProcessLog("Submitting post");
            Post_Rootobject postRootObj = _subredditDo.SubredditSubmit(ranSubRootObj);

            SubredditProcessLog("Submitting comment");
            _subredditDo.SubredditPostComment(ranSubRootObj, postRootObj);
        }

        private void SubredditProcessLog(string logString)
        {
            _logger.LogInformation($"Process - {logString} - {DateTime.Now}");
        }
    }
}
