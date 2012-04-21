using System;
using System.Collections.Generic;
using System.Linq;
using NServiceBus;
using SimpleCqrs.Eventing;

namespace SimpleCqrs.NServiceBus.Eventing
{
   internal class NsbEventBus : IEventBus
   {
      private IBus _bus;

      public void PublishEvent(DomainEvent domainEvent)
      {
         var domainEventMessage = CreateDomainEventMessage(domainEvent, Bus);         
         Bus.Publish(domainEventMessage);
      }

      public void PublishEvents(IEnumerable<DomainEvent> domainEvents)
      {
         domainEvents.ToList().ForEach(PublishEvent);
      }

      private IBus Bus
      {
         get { return _bus ?? (_bus = Configure.Instance.Builder.Build<IBus>()); } //TODO: Injection
      }

      private static IDomainEventMessage CreateDomainEventMessage(DomainEvent domainEvent, IBus bus)
      {
         var domainEventMessageType = MessageCreator.CreateMessageTypeFromDomainEventType(domainEvent.GetType());

         var message = (IDomainEventMessage)bus.CreateInstance(domainEventMessageType);
         message.SetDomainEvent(domainEvent);

         return message;
      }
   }
}