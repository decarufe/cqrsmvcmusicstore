using NServiceBus;
using SimpleCqrs.Commanding;
using SimpleCqrs.NServiceBus.Commanding;

namespace SimpleCqrs.NServiceBus.Commanding
{
   public abstract class CommandWithReturnValueMessageHandler<TCommand> : IHandleMessages<TCommand> where TCommand : SimpleCqrs.NServiceBus.Commanding.ICommandWithReturnValueMessage
   {
      private readonly ICommandBus _commandBus;
      private readonly IBus _bus;

      protected CommandWithReturnValueMessageHandler(ICommandBus commandBus, IBus bus)
      {
         this._commandBus = commandBus;
         this._bus = bus;
      }      

      #region IMessageHandler<TCommand> Members

      public void Handle(TCommand message)
      {
         var value = _commandBus.Execute(message.GetCommand());
         _bus.Return(value);
      }

      #endregion      
   }
}