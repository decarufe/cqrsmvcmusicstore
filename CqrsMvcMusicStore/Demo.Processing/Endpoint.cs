using System.Configuration;
using NServiceBus;
using ProcessingEndpoint.Config;
using SimpleCqrs;
using SimpleCqrs.NServiceBus;
using SimpleCqrs.Unity;

namespace Demo.Processing
{
   public class Endpoint : IConfigureThisEndpoint, AsA_Publisher, IWantCustomInitialization
   {
      void IWantCustomInitialization.Init()
      {
         Configure.With()
            .DefaultBuilder()
            .SimpleCqrs(new SimpleCqrsRuntime<UnityServiceLocator>())
               .UseNsbEventBus()
               .UseLocalCommandBus()
               .UseSqlEventStore(ConfigurationManager.ConnectionStrings["EventStore"].ConnectionString)
            .XmlSerializer()
            .MsmqTransport()
            .UnicastBus();
      }
   }
}
