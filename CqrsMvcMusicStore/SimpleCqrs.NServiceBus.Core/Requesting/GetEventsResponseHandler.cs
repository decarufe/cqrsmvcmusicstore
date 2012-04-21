using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NServiceBus;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using SimpleCqrs.Eventing;

namespace SimpleCqrs.NServiceBus.Requesting
{
   public class GetEventsResponseHandler : IHandleMessages<GetEventsResponse>
   {
      private IEventBus _eventBus;

      public GetEventsResponseHandler() : this(ServiceLocator.Current.Resolve<IEventBus>())
      {         
      }

      public GetEventsResponseHandler(IEventBus eventBus)
      {
         _eventBus = eventBus;         
      }

      #region IMessageHandler<GetEventsResponse> Members

      public void Handle(GetEventsResponse message)
      {
         var bf = new BinaryFormatter();
         IEnumerable<DomainEvent> domainEvents;

         using (var ms = new MemoryStream(message.DomainEventsData))
         {
            domainEvents = (IEnumerable<DomainEvent>)bf.Deserialize(ms);
         }

         _eventBus.PublishEvents(domainEvents);
      }

      #endregion
   }
}
