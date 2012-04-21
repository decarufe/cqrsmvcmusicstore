using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NServiceBus.Unicast.Config;
using System.Reflection;

namespace NServiceBus
{
   public class ConfigMessageSigning : ConfigUnicastBus
   {
      public static bool Sign
      {
         get;
         private set;
      }
      
      public ConfigMessageSigning(ConfigUnicastBus config, bool sign)
      {
         Configurer = config.Configurer;
         Builder = config.Builder;

         Sign = sign;
      }      
   }
}
