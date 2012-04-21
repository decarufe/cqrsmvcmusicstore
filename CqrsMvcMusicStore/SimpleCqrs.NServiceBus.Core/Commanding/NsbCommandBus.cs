using System;
using System.Collections.Generic;
using NServiceBus;
using SimpleCqrs.Commanding;

namespace SimpleCqrs.NServiceBus.Commanding
{
   public class NsbCommandBus : ICommandBus
   {
      private readonly IServiceLocator serviceLocator;
      private readonly IDictionary<Type, string> commandTypeToDestinationLookup;
      private readonly TimeSpan executeTimeout;

      public NsbCommandBus(IServiceLocator serviceLocator, IDictionary<Type, string> commandTypeToDestinationLookup, TimeSpan executeTimeout)
      {
         this.serviceLocator = serviceLocator;
         this.commandTypeToDestinationLookup = commandTypeToDestinationLookup;
         this.executeTimeout = executeTimeout;
      }

      public IBus InnerBus
      {
         get { return serviceLocator.Resolve<IBus>(); }
      }

      public string GetDestinationForCommandType<TCommand>()
      {
         return commandTypeToDestinationLookup[typeof(TCommand)];
      }

      public int Execute<TCommand>(TCommand command) where TCommand : SimpleCqrs.Commanding.ICommand 
      {
         var bus = serviceLocator.Resolve<IBus>();
         var destination = GetDestinationForCommandType<TCommand>();         
         var message = CreateCommandWithReturnValueMessage(command, bus);

         var asyncResult = bus
             .Send(destination, message)
             .Register(state => { }, null);

         if (!asyncResult.AsyncWaitHandle.WaitOne(executeTimeout))
            throw new ExecuteTimeoutException();

         return ((CompletionResult)asyncResult.AsyncState).ErrorCode;
      }

      public void Send<TCommand>(TCommand command) where TCommand : SimpleCqrs.Commanding.ICommand
      {
         var bus = serviceLocator.Resolve<IBus>();
         var destination = commandTypeToDestinationLookup[typeof(TCommand)];
         var message = CreateCommandMessage(command, bus);         

         bus.Send(destination, message);
      }

      private static ICommandMessage CreateCommandMessage(SimpleCqrs.Commanding.ICommand command, IBus bus)
      {
         var commandMessageType = MessageCreator.CreateCommandMessageTypeFromCommandType(command.GetType());

         var message = (ICommandMessage)bus.CreateInstance(commandMessageType);
         message.SetCommand(command);

         return message;
      }

      private static ICommandWithReturnValueMessage CreateCommandWithReturnValueMessage(SimpleCqrs.Commanding.ICommand command, IBus bus)
      {
         var commandMessageType = MessageWithReturnValueCreator.CreateCommandMessageTypeFromCommandType(command.GetType());

         var message = (ICommandWithReturnValueMessage)bus.CreateInstance(commandMessageType);
         message.SetCommand(command);

         return message;
      }
   }
}