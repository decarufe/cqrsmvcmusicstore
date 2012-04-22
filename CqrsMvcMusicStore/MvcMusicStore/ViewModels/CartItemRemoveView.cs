using System.Collections.Generic;
public class CartItemRemoveView
{
   public List<ShoppingCartRemoveItem> Items { get; set; }
}

public class ShoppingCartRemoveItem
{
   public string AlbumName { get; set; }
   public int AlbumId { get; set; }
   public int Count { get; set; }
}
