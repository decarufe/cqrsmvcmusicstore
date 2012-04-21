using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleCqrs.Commanding;

namespace Demo.Commands
{
   [Serializable]
   public class RemoveFromCartCommand : ICommandWithAggregateRootId
   {
      public int Id { get; set; }

      private Guid _aggregateRootID;
      public Guid AggregateRootId
      {
         get { return _aggregateRootID; }
         set { _aggregateRootID = value; }
      }
   }
}
