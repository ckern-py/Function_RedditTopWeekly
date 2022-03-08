using Reddit.MetaData;

namespace Domain
{
    public interface ISubredditDo
    {
        Post_Rootobject SubredditSubmit(Rootobject postFromInfo);
        void SubredditPostComment(Rootobject mainRoot, Post_Rootobject postRoot);
    }
}
