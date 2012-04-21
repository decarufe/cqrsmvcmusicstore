using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleCqrs.Commanding;

namespace Demo.Processing.CommandHandlers
{
   public class ShipItemCommandHandler :
      AggregateRootCommandHandler<Demo.Commands.ShipItemCommand, Demo.Domain.CartItem>
   {
      public override void Handle(Commands.ShipItemCommand command, Domain.CartItem aggregateRoot)
      {
         aggregateRoot.Ship();
      }
   }
}
