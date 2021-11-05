namespace CityInfo.API.Services
{
    public interface IMailService
    {
        void Sent(string subject, string message);

    }
}