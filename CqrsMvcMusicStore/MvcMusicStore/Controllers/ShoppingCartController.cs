using System.Linq;
using System.Web.Mvc;
using MvcMusicStore.Models;
using MvcMusicStore.ViewModels;
using Demo.Commands;
using System.Configuration;
using System.Data.SqlClient;
using System.Collections.Generic;
using System;

namespace MvcMusicStore.Controllers
{
    public class ShoppingCartController : Controller
    {
        MusicStoreEntities storeDB = new MusicStoreEntities();

        //
        // GET: /ShoppingCart/

        public ActionResult Confirmation()
        {
           return View(new object());
        }

        public ActionResult Index()
        {
            var cart = ShoppingCart.GetCart(this.HttpContext);

            // Set up our ViewModel
            //var viewModel = new ShoppingCartViewModel
            //{
            //    CartItems = cart.GetCartItems(),
            //    CartTotal = cart.GetTotal()
            //};

            List<int> albumIds = new List<int>();
            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["CQRS"].ConnectionString))
            {
               connection.Open();

               using (var command = connection.CreateCommand())
               {
                  command.CommandText = "SELECT Id FROM Cart";

                  var reader = command.ExecuteReader();

                  while (reader.Read())
                  {
                     albumIds.Add(reader.GetInt32(0));
                  }

                  reader.Close();
               }

               connection.Close();
            }

           var albums = storeDB.Albums.Where(a => albumIds.Contains(a.AlbumId)).ToList();
         
            var viewModel = new ShoppingCartViewModel
            {
               CartItems = albums.Select(a => new Cart() { Album = a, AlbumId = a.AlbumId, CartId = Guid.NewGuid().ToString(), Count = albumIds.Count(id => id == a.AlbumId), DateCreated = DateTime.Now, RecordId = a.AlbumId }).ToList(),
               CartTotal = albumIds.Count()
            };

            // Return the view
            return View(viewModel);
        }

        //
        // GET: /Store/AddToCart/5

        public ActionResult AddToCart(int id)
        {
           /*
            // Retrieve the album from the database
            var addedAlbum = storeDB.Albums
                .Single(album => album.AlbumId == id);

            // Add it to the shopping cart
            var cart = ShoppingCart.GetCart(this.HttpContext);

            cart.AddToCart(addedAlbum);*/

           var command = new AddToCartCommand() { Id = id };
           MvcApplication.Bus.Send(command);

            // Go back to the main store page for more shopping
            //return RedirectToAction("Index");

           return RedirectToAction("Confirmation");
        }

        //
        // AJAX: /ShoppingCart/RemoveFromCart/5

        [HttpPost]
        public ActionResult RemoveFromCart(int id)
        {
           var aggregateRootID = Guid.Empty;

           using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["CQRS"].ConnectionString))
           {
              connection.Open();

              using (var command = connection.CreateCommand())
              {
                 command.CommandText = "SELECT Top 1 AggregateRootId FROM Cart";

                 aggregateRootID = (Guid)command.ExecuteScalar();
              }

              connection.Close();
           }

           if (aggregateRootID != Guid.Empty)
           {
              var command = new RemoveFromCartCommand() { AggregateRootId = aggregateRootID, Id = id };
              MvcApplication.Bus.Send(command);
           }


            // Remove the item from the cart
            //var cart = ShoppingCart.GetCart(this.HttpContext);

            //// Get the name of the album to display confirmation
            //string albumName = storeDB.Carts
            //    .Single(item => item.RecordId == id).Album.Title;

            // Remove from cart
      //      int itemCount = cart.RemoveFromCart(id);

            // Display the confirmation message
            var results = new ShoppingCartRemoveViewModel
            {
                Message = "Merci d'avoir soumis votre demande...",
                CartTotal = 0,
                CartCount = 0,
                ItemCount = 0,
                DeleteId = id
            };

            return Json(results);
        }

        //
        // GET: /ShoppingCart/CartSummary

        [ChildActionOnly]
        public ActionResult CartSummary()
        {
            var cart = ShoppingCart.GetCart(this.HttpContext);

            ViewData["CartCount"] = cart.GetCount();

            return PartialView("CartSummary");
        }
    }
}