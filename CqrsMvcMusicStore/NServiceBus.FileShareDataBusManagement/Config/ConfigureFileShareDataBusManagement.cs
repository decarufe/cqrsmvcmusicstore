using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NServiceBus.Unicast;
using System.IO;

namespace NServiceBus
{
   public static class ConfigureFileShareDataBusManagement
   {
      public static ConfigFileShareDataBusManagement CleanEmptyFileShareFolders(this Configure config, string basePath)
      {
         return new ConfigFileShareDataBusManagement(config, basePath);
      }      
   }         
}
