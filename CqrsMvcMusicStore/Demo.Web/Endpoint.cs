using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NServiceBus;
using SimpleCqrs.Unity;
using SimpleCqrs;
using SimpleCqrs.NServiceBus;

namespace Demo.Web
{
   public class Endpoint
   {
      public static IStartableBus GetBus()
      {
         return Configure.WithWeb()            
            .DefaultBuilder()            
            .UnicastBus()
            .SimpleCqrs(new SimpleCqrsRuntime<UnityServiceLocator>())
               .UseNsbCommandBus()
            .MsmqTransport()
            .XmlSerializer()
            .CreateBus();            
      }
   }
}
