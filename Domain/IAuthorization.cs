using Reddit.MetaData;

namespace Domain
{
    public interface IAuthorization
    {
        Token GetToken();
    }
}
