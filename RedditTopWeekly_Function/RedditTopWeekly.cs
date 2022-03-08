using Data;
using Domain;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;

namespace RedditTopWeekly_Function
{
    public static class RedditTopWeekly
    {
        [FunctionName("RedditTopWeekly")]
        public static void Run([TimerTrigger("0 0 11,23 * * *")] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"RedditTopWeekly Timer trigger function executed at: {DateTime.Now}");
            try
            {
                ISubredditProcess subredditProcess = new SubredditProcess(log);
                subredditProcess.Process();
            }
            catch (Exception ex)
            {
                log.LogError(ex, "RedditTopWeekly encountered an error");
                IEmail email = new Email(log);
                email.SendEmail(ex);
            }
            finally
            {
                log.LogInformation($"RedditTopWeekly Timer trigger function finished at: {DateTime.Now}");
            }
        }
    }
}
