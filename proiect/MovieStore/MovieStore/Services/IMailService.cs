namespace MovieStore.Services
{
    public interface IMailService
    {
        void Sent(string subject, string message);

    }
}