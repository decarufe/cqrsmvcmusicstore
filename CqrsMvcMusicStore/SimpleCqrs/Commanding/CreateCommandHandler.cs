using SimpleCqrs.Domain;
using SimpleCqrs.Eventing;
using System;
using System.Security;
using Microsoft.IdentityModel.Claims;
using System.Security.Permissions;
using System.Threading;

namespace SimpleCqrs.Commanding
{    
   public abstract class CreateCommandHandler<TCommand> : IHandleCommands<TCommand> where TCommand : ICommand
   {
      private readonly IDomainRepository _domainRepository;
      private readonly IEventStore _eventStore;

      private int ValidationResult { get; set; }

      protected CreateCommandHandler() : this(ServiceLocator.Current.Resolve<IDomainRepository>(), ServiceLocator.Current.Resolve<IEventStore>())
      {
      }

      protected CreateCommandHandler(IDomainRepository domainRepository, IEventStore eventStore)
      {
         _domainRepository = domainRepository;
         _eventStore = eventStore;
      }

      void IHandleCommands<TCommand>.Handle(ICommandHandlingContext<TCommand> handlingContext)
      {
         var command = handlingContext.Command;

         try
         {
            ValidateTheCommand(handlingContext, command);
         }
         catch (SecurityException)
         {           
            if (ValidationResult == 0)
               ValidationResult = 99;
            //TODO : ajouter dans un log
         }
         catch (Exception)
         {
            if (ValidationResult == 0)
               ValidationResult = 99;
         }

         AggregateRoot aggregateRoot = null;

         if (ValidationResult == 0)
         {
            try
            {
               aggregateRoot = Handle(command);               
            }
            catch (SecurityException)
            {
               if (ValidationResult == 0)
                  ValidationResult = 99;
               //TODO : ajouter dans un log
            }
            catch (Exception)
            {
               if (ValidationResult == 0)
                  ValidationResult = 99;
            }
         }

         handlingContext.Return(ValidationResult);

         if (aggregateRoot != null)
            _domainRepository.Save(aggregateRoot);
      }

      private void ValidateTheCommand(ICommandHandlingContext<TCommand> handlingContext, TCommand command)
      {
         //var claimsPrincipal = Thread.CurrentPrincipal as IClaimsPrincipal;

         //if (claimsPrincipal == null)
         //   throw new SecurityException();

         //if (!claimsPrincipal.IsInRole(command.GetType().Name))
         //   throw new SecurityException();

         ValidationResult = IsCommandValid(command, _eventStore) ? 0 : 1;
         handlingContext.Return(ValidationResult);
      }            

      protected virtual bool IsCommandValid(TCommand command, IEventStore eventStore)
      {
         return true;
      }      
      
      protected abstract AggregateRoot Handle(TCommand command);

      protected void Return(int value)
      {
         ValidationResult = value;
      }

      protected void Return(Enum value)
      {
         Return(Convert.ToInt32(value));
      }
   }
}