using NServiceBus;
using SimpleCqrs.Commanding;

namespace SimpleCqrs.NServiceBus.Commanding
{
   public static class CommandBusExtensionMethods
   {
      public static ICallback ExecuteWithCallback<TCommand>(this ICommandBus commandBus, TCommand command) where TCommand : SimpleCqrs.Commanding.ICommand
      {
         var bus = (NsbCommandBus)commandBus;
         var destination = bus.GetDestinationForCommandType<TCommand>();
         var message = CreateCommandWithReturnValueMessage(command, bus.InnerBus);

         return bus.InnerBus.Send(destination, message);         
      }

      public static int ExecuteWeb<TCommand>(this ICommandBus commandBus, TCommand command) where TCommand : SimpleCqrs.Commanding.ICommand
      {
         var bus = (NsbCommandBus)commandBus;
         var destination = bus.GetDestinationForCommandType<TCommand>();
         var message = CreateCommandWithReturnValueMessage(command, bus.InnerBus);
         var returnValue = 0;

         bus.InnerBus.Send(destination, message)
             .Register<int>(errorCode => returnValue = errorCode, null);

         return returnValue;         
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