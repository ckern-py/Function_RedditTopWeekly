# Function_RedditTopWeekly
This is a repo containing an Azure function I created that will submit a post to Reddit every 12 hours. It will also comment on the post it just created.

## Description
This repo contains an Azure function I created that will submit a post to Reddit every 12 hours. It will also comment on the post it just created. The function is set on a timer trigger that runs twice a day. It is set to run at 11AM and 11PM UTC every day. Once kicked off the function interacts with the Reddit APIs to get and set data. The function starts off by going to [www.reddit.com/r/random](www.reddit.com/r/random) and getting redirected to a random subreddit. Once on a random subreddit it sorts the posts and gets the top post of the week. A copy of this post is then submitted to a subreddit I have created specifically for the bot. Once the post is successfully submitted, a comment is then submitted on that post with some stats about the original. Stats such as how popular it was, where it came from, when it was created, and a link to the original post. By having the bot do this it is a great way to get a view of subreddits you don't normally visit.

## Install
Installing this application on your local machine should be simple. You need to make sure you have NET Core Version 3.1 installed. Then you can clone the repo in Visual Studio and open the solution file. 

## Prerequisite
Before being able to run this code you'll need to have a few things set up first. First, you'll need a Reddit account. Next, you'll need to allow the app to have access to your account, or the account you want this running under. After you have done that, you'll need to grant the correct authorization scopes. 

## Use
This project is intended to be hosted and ran in Azure with the supporting infrastructure. You can run it locally for debugging or as a one off if needed. To run in Azure you will need some infrastructure set up with it. You'll need an app service plan and application insight at the very minimum. With it being hosted in Azure it will trigger every time the timer comes up. You won't have to do anything, and information will automatically be submitted to Reddit at regular intervals.

## License
[GNU GPLv3](https://choosealicense.com/licenses/gpl-3.0/)
