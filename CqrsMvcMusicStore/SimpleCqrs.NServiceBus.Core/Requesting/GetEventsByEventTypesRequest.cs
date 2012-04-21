using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NServiceBus;

namespace SimpleCqrs.NServiceBus.Requesting
{
   [Serializable]
   public class GetEventsByEventTypesRequest : IMessage
   {      
      public string[] DomainEventTypes { get; set; }
   }
}
