using FSH.WebApi.Shared.Notifications;

namespace ShortRoute.Client.Infrastructure.Notifications;

public record ConnectionStateChanged(ConnectionState State, string? Message) : INotificationMessage;