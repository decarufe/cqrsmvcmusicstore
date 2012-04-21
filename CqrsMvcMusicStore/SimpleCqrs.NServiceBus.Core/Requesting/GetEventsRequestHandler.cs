using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NServiceBus;
using SimpleCqrs.NServiceBus.Requesting;
using SimpleCqrs.Eventing;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace SimpleCqrs.NServiceBus.Responsing
{
   public class GetEventsRequestHandler :       
      IHandleMessages<GetEventsRequest>,
      IHandleMessages<GetEventsByEventTypesRequest>,
      IHandleMessages<GetEventsByEventTypesForAggregateRootIDRequest>,
      IHandleMessages<GetEventsByEventTypesWithDatesRequest>,
      IHandleMessages<GetEventsForAggregateRootIDRequest>
   {
      public IBus Bus { get; set; }
      public IEventStore EventStore { get; set; }

      public GetEventsRequestHandler() : this(ServiceLocator.Current.Resolve<IBus>(), ServiceLocator.Current.Resolve<IEventStore>())
      {
         
      }

      public GetEventsRequestHandler(IBus bus, IEventStore eventStore)
      {
         Bus = bus;
         EventStore = eventStore;
      }      

      #region IMessageHandler<GetEventsRequest> Members

      public void Handle(GetEventsRequest message)
      {
         var domainEvents = EventStore.GetEvents();
         var response = CreateResponse(domainEvents);

         Bus.Reply(response);
      }

      #endregion

      #region IMessageHandler<GetEventsByEventTypesRequest> Members

      public void Handle(GetEventsByEventTypesRequest message)
      {
         var domainEventTypes = message.DomainEventTypes.Select(e => Type.GetType(e));
         var domainEvents = EventStore.GetEventsByEventTypes(domainEventTypes);

         var response = CreateResponse(domainEvents);

         Bus.Reply(response);
      }      

      #endregion

      #region IMessageHandler<GetEventsByEventTypesForAggregateRootIDRequest> Members

      public void Handle(GetEventsByEventTypesForAggregateRootIDRequest message)
      {
         var domainEventTypes = message.DomainEventTypes.Select(e => Type.GetType(e));
         var domainEvents = EventStore.GetEventsByEventTypes(domainEventTypes, message.AggregateRootId);

         var response = CreateResponse(domainEvents);

         Bus.Reply(response);
      }

      #endregion

      #region IMessageHandler<GetEventsByEventTypesWithDatesRequest> Members

      public void Handle(GetEventsByEventTypesWithDatesRequest message)
      {
         var domainEventTypes = message.DomainEventTypes.Select(e => Type.GetType(e));
         var domainEvents = EventStore.GetEventsByEventTypes(domainEventTypes, message.StartDate, message.EndDate);

         var response = CreateResponse(domainEvents);

         Bus.Reply(response);
      }

      #endregion

      #region IMessageHandler<GetEventsForAggregateRootIDRequest> Members

      public void Handle(GetEventsForAggregateRootIDRequest message)
      {         
         var domainEvents = EventStore.GetEvents(message.AggregateRootId, message.StartSequence);

         var response = CreateResponse(domainEvents);

         Bus.Reply(response);
      }

      #endregion

      private static GetEventsResponse CreateResponse(IEnumerable<DomainEvent> domainEvents)
      {
         var response = new GetEventsResponse();

         var bf = new BinaryFormatter();
         
         using (var ms = new MemoryStream())
         {
            bf.Serialize(ms, domainEvents);
            response.DomainEventsData = ms.ToArray();
         }         
         
         return response;
      }
   }   
}
