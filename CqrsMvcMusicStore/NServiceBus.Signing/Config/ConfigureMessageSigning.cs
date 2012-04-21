using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NServiceBus.Unicast.Config;

namespace NServiceBus
{
   public static class ConfigureMessageSigning
   {
      public static ConfigMessageSigning SignMessages(this ConfigUnicastBus config)
      {
         return SignMessages(config, true);
      }

      public static ConfigMessageSigning SignMessages(this ConfigUnicastBus config, bool sign)
      {
         return new ConfigMessageSigning(config, sign);
      }      
   }
}
