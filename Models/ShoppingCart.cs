using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App_Dev_2.Models
{
    public class ShoppingCart
    {
        [Key]
        public int Id { get; set; }
        public int Count { get; set; }
        
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product Product { get; set; }
        
        public String UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser ApplicationUser{ get; set; }
        
        [NotMapped] public int Price { get; set; }
    }
}