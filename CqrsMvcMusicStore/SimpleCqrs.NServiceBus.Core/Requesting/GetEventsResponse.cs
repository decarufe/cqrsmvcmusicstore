using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NServiceBus;

namespace SimpleCqrs.NServiceBus.Requesting
{
   [Serializable]
   public class GetEventsResponse : IMessage
   {
      public byte[] DomainEventsData { get; set; }
   }
}
