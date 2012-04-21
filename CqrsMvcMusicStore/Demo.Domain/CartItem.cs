﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleCqrs.Domain;
using Demo.Events;

namespace Demo.Domain
{
   public class CartItem : AggregateRoot
   {
      public CartItem()
      {            
      }

      public CartItem(int id)
      {
         base.Id = Guid.NewGuid();
         Apply(new CartItemAddedEvent { Id = id });
      }

      public void Delete()
      {
         Apply(new CartItemRemovedEvent() { AggregateRootId = base.Id });
      }

      public void Ship()
      {
         Apply(new CartItemShippedEvent() { AggregateRootId = base.Id });
      }
   }
}
