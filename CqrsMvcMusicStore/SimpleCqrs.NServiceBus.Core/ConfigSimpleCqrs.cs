using NServiceBus;
using System.Linq;
using SimpleCqrs.Commanding;
using SimpleCqrs.Eventing;
using SimpleCqrs.NServiceBus.Commanding;
using SimpleCqrs.NServiceBus.Eventing;
using NServiceBus.Unicast;
using SimpleCqrs.NServiceBus.Requesting;
using SimpleCqrs.EventStore.SqlServer;
using SimpleCqrs.EventStore.SqlServer.Serializers;

namespace SimpleCqrs.NServiceBus
{
   public class ConfigSimpleCqrs : Configure
   {
      private readonly ISimpleCqrsRuntime runtime;

      public ConfigSimpleCqrs(ISimpleCqrsRuntime runtime)
      {
         this.runtime = runtime;
      }

      public IServiceLocator ServiceLocator { get; private set; }

      public void Configure(Configure config)
      {
         Configurer = config.Configurer;
         Builder = config.Builder;
         
         runtime.Start();                  

         ServiceLocator = runtime.ServiceLocator;
         ServiceLocator.Register(() => Builder.Build<IBus>());         

         Configurer.RegisterSingleton<ISimpleCqrsRuntime>(runtime);
      }

      public ConfigSimpleCqrs UseLocalCommandBus()
      {         
         var config = new ConfigLocalCommandBus(runtime);
         var commandBus = config.Configure(this);         

         ServiceLocator.Register<ICommandBus>(commandBus);
         Configurer.RegisterSingleton<ICommandBus>(commandBus);         

         return this;
      }

      public ConfigSimpleCqrs UseSqlEventStore(string eventStoreConnectionString)
      {         
         var configuration = new SqlServerConfiguration(eventStoreConnectionString);
         IEventStore store = new SqlServerEventStore(configuration, new BinaryDomainEventSerializer());

         runtime.ServiceLocator.Register(store);

         return this;
      }

      public ConfigSimpleCqrs SubscribeForDomainEvents()
      {
         var typeCatalog = ServiceLocator.Resolve<ITypeCatalog>();
         var domainEventHandlerFactory = ServiceLocator.Resolve<DomainEventHandlerFactory>();
         var domainEventTypes = typeCatalog.GetGenericInterfaceImplementations(typeof(IHandleDomainEvents<>));

         var eventBus = new NsbLocalEventBus(domainEventTypes, domainEventHandlerFactory);
         Configurer.RegisterSingleton<IEventBus>(eventBus);         

         var configEventBus = new ConfigLocalEventBus();
         configEventBus.Configure(this, runtime);

         return this;
      }

      public ConfigSimpleCqrs UseNsbCommandBus()
      {
         var config = new ConfigCommandBus(runtime);
         var commandBus = config.Configure(this);

         ServiceLocator.Register<ICommandBus>(commandBus);
         Configurer.RegisterSingleton<ICommandBus>(commandBus);

         return this;
      }

      public ConfigSimpleCqrs UseNsbEventBus()
      {         
         var configEventBus = new ConfigEventBus();
         configEventBus.Configure(this, runtime);

         var eventBus = new NsbEventBus();

         ServiceLocator.Register<IEventBus>(eventBus);
         Configurer.RegisterSingleton<IEventBus>(eventBus);

         return this;
      }      
   }   
}