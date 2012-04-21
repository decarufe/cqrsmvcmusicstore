using NServiceBus;
using System.Collections.Generic;
using System;
using System.Linq;
using ViewModelEndpoint.Config;
using NServiceBus.Unicast;

namespace SimpleCqrs.NServiceBus.Eventing
{   
   public class ConfigLocalEventBus : Configure, IWantToRunAtStartup
   {
      private static readonly IDictionary<Type, Type> _domainEventMessageTypeToDomainEventTypeLookup = new Dictionary<Type, Type>();
      private UnicastBus _bus;
      private Dictionary<Type, string> _domainEventMessageTypeMappings = new Dictionary<Type, string>();

      public static Type GetDomainEventType(Type domainEventMessageType)
      {
         return _domainEventMessageTypeToDomainEventTypeLookup[domainEventMessageType];
      }

      public void Configure(ConfigSimpleCqrs config, ISimpleCqrsRuntime runtime)
      {
         Configurer = config.Configurer;
         Builder = config.Builder;

         var serviceLocator = runtime.ServiceLocator;
         var typeCatalog = serviceLocator.Resolve<ITypeCatalog>();

         var domainEventBusConfig = DomainEventBusConfig.Instance;
         var domainEventTypes = GetManagedDomainEventTypes(typeCatalog);
         var typesToScan = new List<Type>();
         var domainEventTypeMappings = new Dictionary<Type, string>();                  
         _domainEventMessageTypeMappings = new Dictionary<Type, string>();

         RegisterAssemblyDomainEventSubscriptionMappings(domainEventBusConfig, domainEventTypes, _domainEventMessageTypeMappings, domainEventTypeMappings, typesToScan);
         RegisterDomainEventSubscriptionMappings(domainEventBusConfig, domainEventTypes, _domainEventMessageTypeMappings, domainEventTypeMappings, typesToScan);
         RegisterTypes(typesToScan);

         _bus = (global::NServiceBus.Unicast.UnicastBus)config
             .MsmqTransport()            
             //.FileShareDataBus(DataBusConfig.Instance.FileShareDataBus.Path)             
             .UnicastBus()
             .PurgeOnStartup(true)
                 .CreateBus();

         _domainEventMessageTypeMappings.ToList().ForEach(mapping => RegisterMessageType(_bus, mapping));

         var requestBus = new SimpleCqrs.NServiceBus.Requesting.NsbRequestBus(serviceLocator, domainEventTypeMappings);
         serviceLocator.Register<SimpleCqrs.NServiceBus.Requesting.IRequestBus>(requestBus);
         Configurer.RegisterSingleton<SimpleCqrs.NServiceBus.Requesting.IRequestBus>(requestBus);

         _bus.Started += (s, e) =>
            {
               var oldEvents = _domainEventMessageTypeMappings.Keys.Where(t1 => _domainEventMessageTypeMappings.Keys.Any(t2 => t1 != t2 && t1.IsAssignableFrom(t2)));
               var newEvents = _domainEventMessageTypeMappings.Keys.Except(oldEvents);

               newEvents.ToList().ForEach(_bus.Subscribe);
               oldEvents.ToList().ForEach(_bus.Unsubscribe);
            };         
      }      

      private void RegisterDomainEventSubscriptionMappings(DomainEventBusConfig domainEventBusConfig, IEnumerable<Type> domainEventTypes, IDictionary<Type, string> domainEventMessageTypeMappings, IDictionary<Type, string> domainEventTypeMappings, IList<Type> typesToScan)
      {         
         foreach (DomainEventEndpointMapping mapping in domainEventBusConfig.DomainEventEndpointMappings)
         {
            foreach (var domainEventType in domainEventTypes)
            {
               if (DomainEventsIsDomainEvent(domainEventType, mapping.DomainEvents))               
                  RegisterDomainEventSubscriptionMapping(domainEventType, mapping, domainEventMessageTypeMappings, domainEventTypeMappings, typesToScan);               
            }
         }
      }

      private void RegisterAssemblyDomainEventSubscriptionMappings(DomainEventBusConfig domainEventBusConfig, IEnumerable<Type> domainEventTypes, IDictionary<Type, string> domainEventMessageTypeMappings, IDictionary<Type, string> domainEventTypeMappings, IList<Type> typesToScan)
      {
         foreach (DomainEventEndpointMapping mapping in domainEventBusConfig.DomainEventEndpointMappings)
         {
            foreach (var domainEventType in domainEventTypes)
            {
               if (DomainEventsIsAssembly(domainEventType, mapping.DomainEvents))
                  RegisterDomainEventSubscriptionMapping(domainEventType, mapping, domainEventMessageTypeMappings, domainEventTypeMappings, typesToScan);
            }
         }
      }

      private static void RegisterDomainEventSubscriptionMapping(Type domainEventType, DomainEventEndpointMapping mapping, IDictionary<Type, string> domainEventMessageTypeMappings, IDictionary<Type, string> domainEventTypeMappings, IList<Type> typesToScan)
      {
         if (domainEventTypeMappings.ContainsKey(domainEventType))
            domainEventTypeMappings[domainEventType] = mapping.Endpoint;
         else
            domainEventTypeMappings.Add(domainEventType, mapping.Endpoint);

         if (domainEventMessageTypeMappings.ContainsKey(domainEventType))
            domainEventMessageTypeMappings[domainEventType] = mapping.Endpoint;
         else
         {
            var domainEventMessageType = MessageCreator.CreateMessageTypeFromDomainEventType(domainEventType);            
            typesToScan.Add(domainEventMessageType);
            
            domainEventMessageTypeMappings.Add(domainEventMessageType, mapping.Endpoint);
            _domainEventMessageTypeToDomainEventTypeLookup.Add(domainEventMessageType, domainEventType);
         }
      }

      private static bool DomainEventsIsDomainEvent(Type domainEventType, string domainEvents)
      {
         return domainEventType.FullName.ToLower() == domainEvents.ToLower()
                || domainEventType.AssemblyQualifiedName.ToLower() == domainEvents.ToLower();
      }

      private static bool DomainEventsIsAssembly(Type domainEventType, string domainEvents)
      {
         return domainEventType.Assembly.GetName().Name.ToLower() == domainEvents.ToLower();
      }

      private static List<Type> GetManagedDomainEventTypes(ITypeCatalog typeCatalog)
      {
         var domainEventTypes = typeCatalog.GetDerivedTypes(typeof(SimpleCqrs.Eventing.DomainEvent)).ToList();                  

         return domainEventTypes;
      }

      private void RegisterTypes(List<Type> typesToScan)
      {
         ((List<Type>)TypesToScan).AddRange(typesToScan);
      }

      private static void RegisterMessageType(global::NServiceBus.Unicast.UnicastBus bus, KeyValuePair<Type, string> mapping)
      {
         bus.RegisterMessageType(mapping.Key, Address.Parse(mapping.Value));
      }

      public void Run()
      {                       
      }

      public void Stop()
      {
         var oldEvents = _domainEventMessageTypeMappings.Keys.Where(t1 => _domainEventMessageTypeMappings.Keys.Any(t2 => t1 != t2 && t1.IsAssignableFrom(t2)));
         var newEvents = _domainEventMessageTypeMappings.Keys.Except(oldEvents);

         newEvents.ToList().ForEach(_bus.Unsubscribe);
      }
   }   
}