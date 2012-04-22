using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Demo.Events;
using SimpleCqrs.Eventing;

namespace Demo.Tracking.ViewModel
{
   public class CartItemDenormalizer : IHandleDomainEvents<CartItemRemovedEvent>
   {
      private object _locker = new object();
      private static int _removedCount = 0;

      public void Handle(CartItemRemovedEvent domainEvent)
      {
         lock (_locker)
         {
            _removedCount++;
         }


         if (_removedCount == 5)
         {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Envoyer un email au directeur des ventes, il est temps d'offrir un rabais surprise!!!");
            Console.ResetColor();
         }
      }
   }
}
