using Data;
using Domain;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

[assembly: FunctionsStartup(typeof(RedditTopWeekly_Function.Startup))]
namespace RedditTopWeekly_Function
{
    public class Startup : FunctionsStartup
    {
        public IConfigurationRoot Configuration { get; set; }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            var basePath = Directory.GetCurrentDirectory();
            var config = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
            this.Configuration = config;

            builder.Services.AddTransient<IAuthorization, Authorization>();
            builder.Services.AddTransient<ISubredditDo, SubredditDo>();
            builder.Services.AddTransient<ISubredditGet, SubredditGet>();
            builder.Services.AddTransient<ISubredditProcess, SubredditProcess>();
        }
    }
}
