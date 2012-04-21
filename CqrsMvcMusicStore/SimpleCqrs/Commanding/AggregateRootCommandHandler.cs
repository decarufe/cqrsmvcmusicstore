using SimpleCqrs.Domain;
using System.Security;
using System;
using System.Threading;
using Microsoft.IdentityModel.Claims;

namespace SimpleCqrs.Commanding
{
    public abstract class AggregateRootCommandHandler<TCommand, TAggregateRoot> : IHandleCommands<TCommand>
        where TCommand : ICommandWithAggregateRootId
        where TAggregateRoot : AggregateRoot, new()
    {
        private readonly IDomainRepository domainRepository;

        protected AggregateRootCommandHandler() : this(ServiceLocator.Current.Resolve<IDomainRepository>())
        {
        }

        protected AggregateRootCommandHandler(IDomainRepository domainRepository)
        {
            this.domainRepository = domainRepository;
        }

        void IHandleCommands<TCommand>.Handle(ICommandHandlingContext<TCommand> handlingContext)
        {
            var command = handlingContext.Command;

            var aggregateRoot = domainRepository.GetById<TAggregateRoot>(command.AggregateRootId);
            aggregateRoot.Id = command.AggregateRootId;

            try
            {
               ValidateTheCommand(handlingContext, command, aggregateRoot);
            }
            catch (SecurityException)
            {
               ValidationResult = 99;
               //TODO : ajouter dans un log
            }
            catch (Exception)
            {
               ValidationResult = 99;
            }

            if (ValidationResult == 0)
            {
               try
               {
                  Handle(command, aggregateRoot);
               }
               catch (SecurityException)
               {
                  ValidationResult = 99;
                  //TODO : ajouter dans un log
               }
               catch (Exception)
               {
                  ValidationResult = 99;
               }
            }            

            if(aggregateRoot != null)
                domainRepository.Save(aggregateRoot);
        }

        private void ValidateTheCommand(ICommandHandlingContext<TCommand> handlingContext, TCommand command, TAggregateRoot aggregateRoot)
        {
           //var claimsPrincipal = Thread.CurrentPrincipal as IClaimsPrincipal;

           //if (claimsPrincipal == null)
           //   throw new SecurityException();

           //if (!claimsPrincipal.IsInRole(command.GetType().Name))
           //   throw new SecurityException();

            ValidationResult = ValidateCommand(command, aggregateRoot);
            handlingContext.Return(ValidationResult);
        }

        protected int ValidationResult { get; private set; }

        public virtual int ValidateCommand(TCommand command, TAggregateRoot aggregateRoot)
        {
            return 0;
        }

        public abstract void Handle(TCommand command, TAggregateRoot aggregateRoot);
    }
}