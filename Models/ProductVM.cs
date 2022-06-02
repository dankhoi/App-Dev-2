using System.Collections.Generic;
using App_Dev_2.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using App_Dev_2.Models;

namespace App_Dev_2.ViewModels
{
    public class ProductVM
    {
        public Product Product { get; set; }
        public IEnumerable<SelectListItem> CategoryList { get; set; }
    }
}