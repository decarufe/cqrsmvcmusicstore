using System;
using NServiceBus;
using SimpleCqrs.Commanding;

namespace SimpleCqrs.NServiceBus.Commanding
{   
   [Serializable]
   [TimeToBeReceived("00:05:00")]
   public abstract class CommandWithReturnValueMessage : ICommandWithReturnValueMessage
   {
      #region ICommandMessage Members

      public string[] Headers { get; set; }      

      public string CommandTypeName { get; set; }
      
      #endregion
   }
}