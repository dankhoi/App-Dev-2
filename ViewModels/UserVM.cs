using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using App_Dev_2.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace App_Dev_2.ViewModels
{
    public class UserVM
    {
        
        [Required]
        public string Role { get; set; }
        public IEnumerable<SelectListItem> Rolelist { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}