using System;
using NServiceBus;
using System.Xml.Serialization;
using System.IO;

namespace SimpleCqrs.NServiceBus.Commanding
{
   [Serializable]   
   [TimeToBeReceived("00:05:00")]
   public abstract class CommandMessage : ICommandMessage
   {
      #region ICommandMessage Members

      public string[] Headers { get; set; }

      public string CommandTypeName { get; set; }      

      #endregion
   }
}