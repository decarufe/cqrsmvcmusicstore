using System;
using System.Collections.Generic;
namespace SimpleCqrs.NServiceBus.Requesting
{
   public interface IRequestBus
   {      
      void GetEvents(IEnumerable<string> destination);      
      void GetEvents(IEnumerable<string> destination, Guid aggregateRootID, int startSequence);
      void GetEventsByEventTypes(IEnumerable<Type> types);
      void GetEventsByEventTypes(IEnumerable<Type> types, DateTime startDate, DateTime endDate);
      void GetEventsByEventTypes(IEnumerable<Type> types, Guid aggregateRootID);
   }
}
