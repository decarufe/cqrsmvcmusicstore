using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleCqrs.Commanding;

namespace Demo.Commands
{
   [Serializable]
   public class AddToCartCommand : ICommand
   {
      public int Id { get; set; }
   }
}
