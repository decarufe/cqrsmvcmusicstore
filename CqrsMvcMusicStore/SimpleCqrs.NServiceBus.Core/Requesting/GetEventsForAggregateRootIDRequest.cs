using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NServiceBus;

namespace SimpleCqrs.NServiceBus.Requesting
{
   [Serializable]
   public class GetEventsForAggregateRootIDRequest : IMessage
   {
      public Guid AggregateRootId { get; set; }
      public int StartSequence { get; set; }
   }   
}
