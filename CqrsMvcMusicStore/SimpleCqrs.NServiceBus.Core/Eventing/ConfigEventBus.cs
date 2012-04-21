using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NServiceBus;
using SimpleCqrs.Eventing;

namespace SimpleCqrs.NServiceBus.Eventing
{
   public class ConfigEventBus : Configure
   {  
      public void Configure(ConfigSimpleCqrs config, ISimpleCqrsRuntime runtime)      
      {
         Configurer = config.Configurer;
         Builder = config.Builder;

         var serviceLocator = runtime.ServiceLocator;
         var typeCatalog = serviceLocator.Resolve<ITypeCatalog>();
         var domainEventTypes = typeCatalog.GetDerivedTypes(typeof(DomainEvent));
         var typesToScan = new List<Type>();

         RegisterDomainEventMessages(domainEventTypes, typesToScan);
         RegisterTypes(typesToScan);         
      }      

      private static void RegisterDomainEventMessages(Type[] domainEventTypes, List<Type> typesToScan)
      {
         typesToScan.AddRange(domainEventTypes.Select(domainEventType => MessageCreator.CreateMessageTypeFromDomainEventType(domainEventType)));
      }

      private void RegisterTypes(List<Type> typesToScan)
      {
         ((List<Type>)TypesToScan).AddRange(typesToScan);
      }
   }
}
