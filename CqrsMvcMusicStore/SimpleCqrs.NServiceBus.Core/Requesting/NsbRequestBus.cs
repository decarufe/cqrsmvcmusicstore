using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleCqrs.Commanding;
using NServiceBus;

namespace SimpleCqrs.NServiceBus.Requesting
{
   public class NsbRequestBus : IRequestBus
   {
      private readonly IServiceLocator serviceLocator;
      private readonly IDictionary<Type, string> _domainEventTypeToDestinationLookup;

      public NsbRequestBus(IServiceLocator serviceLocator, IDictionary<Type, string> domainEventTypeToDestinationLookup)
      {
         this.serviceLocator = serviceLocator;
         this._domainEventTypeToDestinationLookup = domainEventTypeToDestinationLookup;
      }

      public IBus InnerBus
      {
         get { return serviceLocator.Resolve<IBus>(); }
      }      

      public void GetEvents(IEnumerable<string> destinations, Guid aggregateRootID, int startSequence)
      {         
         destinations.ToList().ForEach(destination => InnerBus.Send<GetEventsForAggregateRootIDRequest>(destination, message => 
            {
               message.AggregateRootId = aggregateRootID;
               message.StartSequence = startSequence;
            }));         
      }  

      public void GetEvents(IEnumerable<string> destination)
      {
         destination.ToList().ForEach(d => InnerBus.Send<GetEventsRequest>(d, message => { }));
      }

      public void GetEventsByEventTypes(IEnumerable<Type> types)
      {         
         var destinationToTypes = new Dictionary<string, List<Type>>();

         types.ToList().ForEach(t =>
            {
               if (!destinationToTypes.ContainsKey(_domainEventTypeToDestinationLookup[t]))
                  destinationToTypes.Add(_domainEventTypeToDestinationLookup[t], new List<Type>());

               destinationToTypes[_domainEventTypeToDestinationLookup[t]].Add(t);
            });

         destinationToTypes.ToList().ForEach(d =>
            {
               InnerBus.Send<GetEventsByEventTypesRequest>(d.Key, message =>
               {
                  message.DomainEventTypes = d.Value.ConvertAll(t => t.AssemblyQualifiedName).ToArray();
               });
            });
      }

      public void GetEventsByEventTypes(IEnumerable<Type> types, Guid aggregateRootID)
      {
         var destinationToTypes = new Dictionary<string, List<Type>>();
         
         types.ToList().ForEach(t =>
            {
               if (!destinationToTypes.ContainsKey(_domainEventTypeToDestinationLookup[t]))
                  destinationToTypes.Add(_domainEventTypeToDestinationLookup[t], new List<Type>());

               destinationToTypes[_domainEventTypeToDestinationLookup[t]].Add(t);
            });

         destinationToTypes.ToList().ForEach(d =>
            {
               InnerBus.Send<GetEventsByEventTypesForAggregateRootIDRequest>(d.Key, message =>
                  {
                     message.DomainEventTypes = d.Value.ConvertAll(t => t.AssemblyQualifiedName).ToArray();
                     message.AggregateRootId = aggregateRootID;
                  });
            });
      }

      public void GetEventsByEventTypes(IEnumerable<Type> types, DateTime startDate, DateTime endDate)
      {                  
         var destinationToTypes = new Dictionary<string, List<Type>>();
         
         types.ToList().ForEach(t =>
            {
               if (!destinationToTypes.ContainsKey(_domainEventTypeToDestinationLookup[t]))
                  destinationToTypes.Add(_domainEventTypeToDestinationLookup[t], new List<Type>());

               destinationToTypes[_domainEventTypeToDestinationLookup[t]].Add(t);
            });

         destinationToTypes.ToList().ForEach(d =>
            {
               InnerBus.Send<GetEventsByEventTypesWithDatesRequest>(d.Key, message =>
                  {
                     message.DomainEventTypes = d.Value.ConvertAll(t => t.AssemblyQualifiedName).ToArray();
                     message.StartDate = startDate;
                     message.EndDate = endDate;
                  });
            });
      }          
   }
}
