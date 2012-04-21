using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NServiceBus;

namespace SimpleCqrs.NServiceBus.Requesting
{
   [Serializable]
   public class GetEventsByEventTypesWithDatesRequest : IMessage
   {
      public string[] DomainEventTypes { get; set; }
      public DateTime StartDate { get; set; }
      public DateTime EndDate { get; set; }      
   }
}
