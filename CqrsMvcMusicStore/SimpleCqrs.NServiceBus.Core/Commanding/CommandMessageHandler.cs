using NServiceBus;
using SimpleCqrs.NServiceBus.Commanding;

namespace SimpleCqrs.NServiceBus
{
   public abstract class CommandMessageHandler<TCommand> : IHandleMessages<TCommand> where TCommand : SimpleCqrs.NServiceBus.Commanding.ICommandMessage
   {
      private readonly SimpleCqrs.Commanding.ICommandBus _commandBus;

      protected CommandMessageHandler(SimpleCqrs.Commanding.ICommandBus commandBus)
      {
         this._commandBus = commandBus;
      }

      public void Handle(TCommand message)
      {
         _commandBus.Send(message.GetCommand());
      }
   }
}