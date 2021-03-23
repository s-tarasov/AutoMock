using System.IO;

namespace ExampleApp
{
    public interface INotificationRepository
    {
        bool SetShown();
    }

    public class NotificationRepository : INotificationRepository
    {
        private const string FileName = "notification.lock";

        public bool SetShown()
        {
            if (File.Exists(FileName))
                return false;

            File.Create(FileName);

            return true;
        }
    }
}
