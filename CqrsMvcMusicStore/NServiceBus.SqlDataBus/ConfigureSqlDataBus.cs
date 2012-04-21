using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NServiceBus.DataBus;

namespace NServiceBus
{
   public static class ConfigureSqlDataBus
   {
      public static Configure SqlDataBus(this Configure config, string connectionString)
      {
         var dataBus = new SqlDataBus(connectionString);

         config.Configurer.RegisterSingleton<IDataBus>(dataBus);

         return config;
      }
   }
}
