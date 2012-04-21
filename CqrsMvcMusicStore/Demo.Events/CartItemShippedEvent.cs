using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleCqrs.Eventing;

namespace Demo.Events
{
   [Serializable]
   public class CartItemShippedEvent : DomainEvent
   {
   }
}
