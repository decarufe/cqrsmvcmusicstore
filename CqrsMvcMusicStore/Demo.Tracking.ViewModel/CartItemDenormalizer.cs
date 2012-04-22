using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Demo.Events;
using SimpleCqrs.Eventing;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace Demo.Tracking.ViewModel
{
   public class CartItemDenormalizer : IHandleDomainEvents<CartItemRemovedEvent>
   {
      public void Handle(CartItemRemovedEvent domainEvent)
      {
         using (var connexion = new SqlConnection(ConfigurationManager.ConnectionStrings["CQRS"].ConnectionString))
         {
            connexion.Open();

            using (var command = connexion.CreateCommand())
            {
               command.CommandText = "UPDATE Retrait SET Count = Count + 1 WHERE ID = @ID IF @@ROWCOUNT = 0 INSERT INTO Retrait (Id, Count) values(@ID, 0)";
               command.Parameters.Add("@ID", SqlDbType.Int).Value = domainEvent.Id;

               command.ExecuteNonQuery();
            }

            connexion.Close();
         }
      }
   }
}
