using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleCqrs.Commanding;

namespace Demo.Processing.CommandHandlers
{
   public class AddToCartCommandHandler
      : CreateCommandHandler<Demo.Commands.AddToCartCommand>
   {
      protected override SimpleCqrs.Domain.AggregateRoot Handle(Demo.Commands.AddToCartCommand command)
      {
         var cartItem = new Demo.Domain.CartItem(command.Id);
         
         return cartItem;
      }
   }
}
