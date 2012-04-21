using System;
using System.Collections.Generic;
using System.Linq;
using NServiceBus;
using SimpleCqrs.Commanding;
using CommandEndpoint.Config;

namespace SimpleCqrs.NServiceBus.Commanding
{
   public class ConfigCommandBus : ConfigSimpleCqrs
   {
      private readonly ISimpleCqrsRuntime runtime;
      private readonly IDictionary<Type, string> commandTypeToDestinationLookup = new Dictionary<Type, string>();
      private readonly IDictionary<Type, Type> commandTypeToCommandMessageHandlerLookup = new Dictionary<Type, Type>();

      public ConfigCommandBus(ISimpleCqrsRuntime runtime)
         : base(runtime)
      {
         this.runtime = runtime;
      }

      public IDictionary<Type, string> CommandTypeToDestinationLookup
      {
         get { return commandTypeToDestinationLookup; }
      }

      public TimeSpan ExecuteCommandTimeout { get; set; }

      public new NsbCommandBus Configure(Configure config)
      {
         Configurer = config.Configurer;
         Builder = config.Builder;

         ExecuteCommandTimeout = TimeSpan.FromSeconds(10);
         var commandBusConfig = CommandBusConfig.Instance;
         var commandTypes = GetManagedCommandTypes();
         var typesToScan = new List<Type>();

         RegisterAssemblyCommandDestinationMappings(commandBusConfig, commandTypes, typesToScan);
         RegisterCommandDestinationMappings(commandBusConfig, commandTypes, typesToScan);
         RegisterTypes(typesToScan);

         return new NsbCommandBus(runtime.ServiceLocator, CommandTypeToDestinationLookup, ExecuteCommandTimeout);
      }

      private void RegisterCommandDestinationMappings(CommandBusConfig commandBusConfig, IEnumerable<Type> commandTypes, IList<Type> typesToScan)
      {
         foreach (var mapping in GetCommandEndpointMappingsForCommand(commandBusConfig.CommandEndpointMappings, commandTypes))
         {
            foreach (var commandType in commandTypes)
            {
               if (CommandTypeIsCommand(commandType, mapping.Commands))
                  RegisterCommandDestinationMapping(commandType, mapping, typesToScan);
            }
         }
      }

      private void RegisterAssemblyCommandDestinationMappings(CommandBusConfig commandBusConfig, IEnumerable<Type> commandTypes, IList<Type> typesToScan)
      {
         foreach (var mapping in GetCommandEndpointMappingsForAssembly(commandBusConfig.CommandEndpointMappings, commandTypes))
         {
            foreach (var commandType in commandTypes)
            {
               if (CommandTypeIsInCommandAssembly(commandType, mapping.Commands))
                  RegisterCommandDestinationMapping(commandType, mapping, typesToScan);
            }
         }
      }

      private void RegisterCommandDestinationMapping(Type commandType, CommandEndpointMapping mapping, IList<Type> typesToScan)
      {
         if (commandTypeToDestinationLookup.ContainsKey(commandType))
            commandTypeToDestinationLookup[commandType] = mapping.Endpoint;
         else
         {
            var commandMessageType = MessageCreator.CreateCommandMessageTypeFromCommandType(commandType);
            var commandWithReturnValueMessageType = MessageWithReturnValueCreator.CreateCommandMessageTypeFromCommandType(commandType);

            typesToScan.Add(commandMessageType);
            typesToScan.Add(commandWithReturnValueMessageType);
            commandTypeToDestinationLookup.Add(commandType, mapping.Endpoint);
         }
      }

      private static IEnumerable<CommandEndpointMapping> GetCommandEndpointMappingsForAssembly(CommandEndpointMappings commandEndpointMappings, IEnumerable<Type> commandTypes)
      {
         return commandEndpointMappings
             .Cast<CommandEndpointMapping>()
             .Where(mapping => commandTypes.Any(t => t.Assembly.GetName().Name.Equals(mapping.Commands, StringComparison.InvariantCultureIgnoreCase)));
      }

      private static IEnumerable<CommandEndpointMapping> GetCommandEndpointMappingsForCommand(CommandEndpointMappings commandEndpointMappings, IEnumerable<Type> commandTypes)
      {
         return commandEndpointMappings
             .Cast<CommandEndpointMapping>()
             .Where(mapping => commandTypes.Any(t => t.FullName.Equals(mapping.Commands, StringComparison.InvariantCultureIgnoreCase) || t.Name.Equals(mapping.Commands, StringComparison.InvariantCultureIgnoreCase)));
      }

      private static bool CommandTypeIsInCommandAssembly(Type commandType, string commandAssembly)
      {
         return commandType.Assembly.GetName().Name.ToLower() == commandAssembly.ToLower();
      }

      private static bool CommandTypeIsCommand(Type commandType, string command)
      {
         return commandType.FullName.ToLower() == command.ToLower()
                || commandType.AssemblyQualifiedName.ToLower() == command.ToLower();
      }

      private static List<Type> GetManagedCommandTypes()
      {
         var commandTypes = TypesToScan
             .Where(type => typeof(SimpleCqrs.Commanding.ICommand).IsAssignableFrom(type))
             .ToList();

         return commandTypes;
      }

      private void RegisterTypes(List<Type> typesToScan)
      {
         ((List<Type>)TypesToScan).AddRange(typesToScan);
      }
   }
}