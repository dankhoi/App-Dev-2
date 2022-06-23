using System.Collections;
using System.Collections.Generic;

namespace App_Dev_2.Models
{
    public class ShoppingCartVM
    {
        public IEnumerable<ShoppingCart> ListCarts { get; set; }
        public OrderHeader OrderHeader { get; set; }
    }
}