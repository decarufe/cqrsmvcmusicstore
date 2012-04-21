using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleCqrs;
using SimpleCqrs.NServiceBus;
using SimpleCqrs.Unity;
using NServiceBus;
using SimpleCqrs.Commanding;
using Demo.Commands;
using System.Data.SqlClient;
using System.Configuration;

namespace ShippingConsole
{
   class Program
   {
      static void Main(string[] args)
      {
         SimpleCqrsRuntime<UnityServiceLocator> runtime = new SimpleCqrsRuntime<UnityServiceLocator>();

         var config = Configure.With()
            .DefaultBuilder()
            .UnicastBus()
            .SimpleCqrs(runtime)
               .UseNsbCommandBus()
            .MsmqTransport()
            .XmlSerializer();

         config.CreateBus().Start();

         var commandBus = runtime.ServiceLocator.Resolve<ICommandBus>();
         string input;
         while (true)
         {
            input = Console.ReadLine();

            if (input.StartsWith("l", StringComparison.OrdinalIgnoreCase))
            {
               PrintPendingCommand();
            }
            else if (input.StartsWith("s", StringComparison.OrdinalIgnoreCase))
            {
               ShipNext(commandBus);
            }
            else if (input.StartsWith("q", StringComparison.OrdinalIgnoreCase))
            {
               break;
            }
         }

         runtime.Shutdown();
      }

      private static void ShipNext(ICommandBus commandBus)
      {
         List<Guid> ids = GetIds();

         if (ids.Count() == 0)
            Console.WriteLine("Aucune commandes en attente");
         else
         {
            Guid id = ids.First();

            var cmd = new ShipItemCommand() { AggregateRootId = id };
            commandBus.Send(cmd);

            Console.WriteLine("Un email à été envoyé pour la commande {0}", id.ToString());
         }
      }

      private static void PrintPendingCommand()
      {
         List<Guid> ids = GetIds();

         if (ids.Count() == 0)
            Console.WriteLine("Aucune commandes en attente");
         else
            ids.ForEach(id => Console.WriteLine(id));
      }

      private static List<Guid> GetIds()
      {
         List<Guid> ids = new List<Guid>();

         using (var connexion = new SqlConnection(ConfigurationManager.ConnectionStrings["CQRS"].ConnectionString))
         {
            connexion.Open();

            using (var command = connexion.CreateCommand())
            {
               command.CommandText = "SELECT AggregateRootId FROM ToShip";

               var reader = command.ExecuteReader();

               while (reader.Read())
                  ids.Add((Guid)reader[0]);
            }

            connexion.Close();
         }
         return ids;
      }
   }
}
