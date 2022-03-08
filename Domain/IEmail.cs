using System;

namespace Domain
{
    public interface IEmail
    {
        void SendEmail(Exception exception);
    }
}
