using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NServiceBus;

namespace SimpleCqrs.NServiceBus.Requesting
{
   [Serializable]
   public class GetEventsByEventTypesForAggregateRootIDRequest : IMessage
   {
      public string[] DomainEventTypes { get; set; }
      public Guid AggregateRootId { get; set; }      
   }
}
