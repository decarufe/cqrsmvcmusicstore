using NServiceBus;
using SimpleCqrs;
using SimpleCqrs.NServiceBus;
using SimpleCqrs.Unity;
using ViewModelEndpoint.Config;

namespace Demo.ViewModel
{
   public class Endpoint : IConfigureThisEndpoint, AsA_Client, IWantCustomInitialization
   {
      void IWantCustomInitialization.Init()
      {
         Configure.With()
            .DefaultBuilder()
            .SimpleCqrs(new SimpleCqrsRuntime<UnityServiceLocator>())
               .SubscribeForDomainEvents()
            .MsmqTransport()
            .XmlSerializer()            
            .UnicastBus();            
      }
   }
}
