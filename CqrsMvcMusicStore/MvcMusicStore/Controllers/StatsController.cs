using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Configuration;
using MvcMusicStore.Models;

namespace MvcMusicStore.Controllers
{
    public class StatsController : Controller
    {
        //
        // GET: /Stats/

        public ActionResult Index()
        {
           List<ShoppingCartRemoveItem> retraits = new List<ShoppingCartRemoveItem>();
           using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["CQRS"].ConnectionString))
           {
              connection.Open();

              using (var command = connection.CreateCommand())
              {
                 command.CommandText = "SELECT Id, Count FROM Retrait ORDER BY Count DESC";

                 var reader = command.ExecuteReader();

                 while (reader.Read())
                 {
                    retraits.Add(new ShoppingCartRemoveItem { AlbumId = reader.GetInt32(0), Count = reader.GetInt32(1) });
                 }

                 reader.Close();
              }

              connection.Close();
           }

           MusicStoreEntities storeDB = new MusicStoreEntities();
           
           retraits.ForEach(r => {
              r.AlbumName = storeDB.Albums.First(a => a.AlbumId == r.AlbumId).Title;
           });

           var viewModel = new CartItemRemoveView
           {
              Items = retraits
           };


            return View(viewModel);
        }

    }
}
