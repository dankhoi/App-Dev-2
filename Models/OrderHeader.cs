using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App_Dev_2.Models
{
    public class OrderHeader
    {
        [Key] public int Id { get; set; }

        public int Total { get; set; }
        [Required] public string Address { get; set; }
        public DateTime OrderDate { get; set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")] public ApplicationUser ApplicationUser { get; set; }
        
    }
}