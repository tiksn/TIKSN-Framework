using MediatR;

namespace TIKSN.Integration.Messages.Events;

#pragma warning disable CA1040 // Marker interface for framework event messages.
public interface IEvent : INotification;
#pragma warning restore CA1040 // Marker interface for framework event messages.
