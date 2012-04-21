using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleCqrs.Eventing;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace Demo.Shipping.ViewModel
{
   public class CartItemDenormalizer :
      IHandleDomainEvents<Demo.Events.CartItemAddedEvent>,
      IHandleDomainEvents<Demo.Events.CartItemRemovedEvent>,
      IHandleDomainEvents<Demo.Events.CartItemShippedEvent>
   {
      public void Handle(Events.CartItemAddedEvent domainEvent)
      {
         AddItem(domainEvent.AggregateRootId);
      }

      public void Handle(Events.CartItemRemovedEvent domainEvent)
      {
         RemoveItem(domainEvent.AggregateRootId);
      }

      public void Handle(Events.CartItemShippedEvent domainEvent)
      {
         RemoveItem(domainEvent.AggregateRootId);
      }

      private void AddItem(Guid guid)
      {
         using (var connexion = new SqlConnection(ConfigurationManager.ConnectionStrings["CQRS"].ConnectionString))
         {
            connexion.Open();

            using (var command = connexion.CreateCommand())
            {
               command.CommandText = "INSERT INTO ToShip (AggregateRootId) Values(@ARID)";
               command.Parameters.Add("@ARID", SqlDbType.UniqueIdentifier).Value = guid;

               command.ExecuteNonQuery();
            }

            connexion.Close();
         }
      }

      private void RemoveItem(Guid guid)
      {
         using (var connexion = new SqlConnection(ConfigurationManager.ConnectionStrings["CQRS"].ConnectionString))
         {
            connexion.Open();

            using (var command = connexion.CreateCommand())
            {
               command.CommandText = "DELETE FROM ToShip WHERE AggregateRootId = @ARID";
               command.Parameters.Add("@ARID", SqlDbType.UniqueIdentifier).Value = guid;

               command.ExecuteNonQuery();
            }

            connexion.Close();
         }
      }
   }
}
