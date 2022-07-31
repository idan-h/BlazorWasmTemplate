using FSH.WebApi.Shared.Notifications;

namespace ShortRoute.Client.Infrastructure.Notifications;

public interface INotificationPublisher
{
    Task PublishAsync(INotificationMessage notification);
}