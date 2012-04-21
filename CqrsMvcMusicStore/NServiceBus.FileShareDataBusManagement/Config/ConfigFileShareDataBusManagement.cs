using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NServiceBus
{
   public class ConfigFileShareDataBusManagement
   {
      private Configure _config;

      public ConfigFileShareDataBusManagement(Configure config, string basePath)
      {         
         _config = config;
                  
         FileShareDataBusManager.BasePath = basePath;
      }      

      public Configure Periodically()
      {
         FileShareDataBusManager.Interval = TimeSpan.FromDays(1);         

         return _config;
      }

      public Configure Every(TimeSpan delay)
      {
         FileShareDataBusManager.Interval = delay;

         return _config;
      }
   }
}
