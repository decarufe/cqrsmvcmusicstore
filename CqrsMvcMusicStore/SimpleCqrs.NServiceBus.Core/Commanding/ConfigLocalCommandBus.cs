using System;
using System.Collections.Generic;
using System.Linq;
using NServiceBus;
using SimpleCqrs.Commanding;
using ProcessingEndpoint.Config;

namespace SimpleCqrs.NServiceBus.Commanding
{
   public class ConfigLocalCommandBus : ConfigSimpleCqrs
   {
      private readonly ISimpleCqrsRuntime _runtime;

      public ConfigLocalCommandBus(ISimpleCqrsRuntime runtime)
         : base(runtime)
      {
         this._runtime = runtime;
      }

      public new LocalCommandBus Configure(Configure config)
      {
         Configurer = config.Configurer;
         Builder = config.Builder;

         var localCommandBus = _runtime.ServiceLocator.Resolve<LocalCommandBus>();

         var commandBusConfig = CommandBusConfig.Instance;
         var commandMessageHandlerTypes = new List<Type>();
         var commandTypes = GetManagedCommandTypes();
         var typesToScan = new List<Type>();

         RegisterAssemblyCommandMessageHandlers(commandBusConfig, commandTypes, commandMessageHandlerTypes, typesToScan);
         RegisterCommandMessageHandlers(commandBusConfig, commandTypes, commandMessageHandlerTypes, typesToScan);
         RegisterTypes(typesToScan);

         return localCommandBus;
      }

      private void RegisterCommandMessageHandlers(CommandBusConfig commandBusConfig, IEnumerable<Type> commandTypes, List<Type> commandMessageHandlerTypes, IList<Type> typesToScan)
      {
         foreach (var handler in GetCommandHandlersForCommand(commandBusConfig.CommandHandlers, commandTypes))
         {
            foreach (var commandType in commandTypes)
            {
               if (CommandTypeIsCommand(commandType, handler.Commands))
                  RegisterCommandMessageHandler(commandType, commandMessageHandlerTypes, typesToScan);
            }
         }
      }

      private void RegisterAssemblyCommandMessageHandlers(CommandBusConfig commandBusConfig, IEnumerable<Type> commandTypes, List<Type> commandMessageHandlerTypes, IList<Type> typesToScan)
      {
         foreach (var mapping in GetCommandHandlersForAssembly(commandBusConfig.CommandHandlers, commandTypes))
         {
            foreach (var commandType in commandTypes)
            {
               if (CommandTypeIsInCommandAssembly(commandType, mapping.Commands))
                  RegisterCommandMessageHandler(commandType, commandMessageHandlerTypes, typesToScan);
            }
         }
      }

      private static void RegisterCommandMessageHandler(Type commandType, List<Type> commandMessageHandlerTypes, IList<Type> typesToScan)
      {
         if (commandMessageHandlerTypes.Contains(commandType))
            return;

         var commandMessageType = MessageCreator.CreateCommandMessageTypeFromCommandType(commandType);
         var commandWithReturnValueMessageType = MessageWithReturnValueCreator.CreateCommandMessageTypeFromCommandType(commandType);
         var commandMessageHandlerType = MessageHandlerCreator.CreateCommandMessageHandlerForCommandMessageType(commandMessageType);
         var commandWithReturnValueMessageHandlerType = MessageWithReturnValueHandlerCreator.CreateCommandMessageHandlerForCommandMessageType(commandWithReturnValueMessageType);

         typesToScan.Add(commandMessageType);
         typesToScan.Add(commandMessageHandlerType);
         typesToScan.Add(commandWithReturnValueMessageType);
         typesToScan.Add(commandWithReturnValueMessageHandlerType);                           

         commandMessageHandlerTypes.Add(commandMessageHandlerType);
      }

      private static IEnumerable<CommandHandler> GetCommandHandlersForAssembly(CommandHandlers commandHandlers, IEnumerable<Type> commandTypes)
      {
         return commandHandlers
             .Cast<CommandHandler>()
             .Where(handler => commandTypes.Any(t => t.Assembly.GetName().Name.Equals(handler.Commands, StringComparison.InvariantCultureIgnoreCase)));
      }

      private static IEnumerable<CommandHandler> GetCommandHandlersForCommand(CommandHandlers commandHandlers, IEnumerable<Type> commandTypes)
      {
         return commandHandlers
             .Cast<CommandHandler>()
             .Where(handler => commandTypes.Any(t => t.FullName.Equals(handler.Commands, StringComparison.InvariantCultureIgnoreCase) || t.Name.Equals(handler.Commands, StringComparison.InvariantCultureIgnoreCase)));
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