using NServiceBus;
using SimpleCqrs.Eventing;

namespace SimpleCqrs.NServiceBus.Eventing
{     
   public class DomainEventMessageHandler : IHandleMessages<IDomainEventMessage>
   {
      private readonly SimpleCqrs.Eventing.IEventBus _eventBus;

      public DomainEventMessageHandler(SimpleCqrs.Eventing.IEventBus eventBus)
      {
         this._eventBus = eventBus;
      }

      #region IMessageHandler<IDomainEventMessage> Members

      public void Handle(IDomainEventMessage message)
      {
         _eventBus.PublishEvent(message.GetDomainEvent());
      }

      #endregion
   }
}