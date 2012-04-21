using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleCqrs.Eventing;
using System.Data.SqlClient;
using System.Data;

namespace Demo.Denormalizers
{
   public class CartItemDenormalizer : 
      IHandleDomainEvents<Demo.Events.CartItemAddedEvent>,
      IHandleDomainEvents<Demo.Events.CartItemRemovedEvent>
   {
      private static string _connectionString = @"Server=.\sqlexpress;Database=DEMO_MVC;Trusted_Connection=True;";
      public void Handle(Events.CartItemAddedEvent domainEvent)
      {         
         using (var connexion = new SqlConnection(_connectionString))
         {
            connexion.Open();
   
            using (var command = connexion.CreateCommand())
            {
               command.CommandText = "INSERT INTO Cart (AggregateRootId, Id) Values(@ARID, @Id)";
               command.Parameters.Add("@Id", SqlDbType.Int).Value = domainEvent.Id;
               command.Parameters.Add("@ARID", SqlDbType.UniqueIdentifier).Value = domainEvent.AggregateRootId;
               
               command.ExecuteNonQuery();
            }

            connexion.Close();
         }

         Console.ForegroundColor = ConsoleColor.Cyan;
         Console.WriteLine("Item ajouté, id: {0}", domainEvent.Id);
         Console.ResetColor();
      }

      public void Handle(Events.CartItemRemovedEvent domainEvent)
      {
         using (var connexion = new SqlConnection(_connectionString))
         {
            connexion.Open();

            using (var command = connexion.CreateCommand())
            {
               command.CommandText = "delete from Cart where aggregaterootid = @Id";
               command.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = domainEvent.AggregateRootId;

               command.ExecuteNonQuery();
            }

            connexion.Close();
         }

         Console.ForegroundColor = ConsoleColor.Cyan;
         Console.WriteLine("Item supprimé, id: {0}", domainEvent.Id);
         Console.ResetColor();
      }
   }
}
