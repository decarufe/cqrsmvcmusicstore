using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleCqrs.Commanding;

namespace Demo.Processing.CommandHandlers
{
   public class RemoveFromCartCommandHandler :
      AggregateRootCommandHandler<Demo.Commands.RemoveFromCartCommand, Demo.Domain.CartItem>
   {
      public override void Handle(Commands.RemoveFromCartCommand command, Domain.CartItem aggregateRoot)
      {
         aggregateRoot.Delete();
      }
   }
}
