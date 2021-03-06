using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using App_Dev_2.Data;
using App_Dev_2.Models;
using App_Dev_2.Data;
using App_Dev_2.Models;
using App_Dev_2.Utility;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace App_Dev_2.Areas.Authenticated.Controllers
{
    [Area("Authenticated")]
    [Authorize(Roles = SD.Role_StoreOwner)]
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CategoriesController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET
        public IActionResult Index(string searchString)
        {
            var listCategories = _db.Categories.ToList();
            if (!String.IsNullOrEmpty(searchString))
            {
                listCategories = listCategories.Where(c => c.Name.Contains(searchString)).ToList();
            }

            return View(listCategories);
        }

        [HttpGet]
        public IActionResult Upsert(int? id)
        {
            if (id == null)
            {
                return View(new Category());
            }

            var findCategory = _db.Categories.Find(id);

            return View(findCategory);
        }

        [HttpPost]
        public IActionResult Upsert(Category category)
        {
            if (ModelState.IsValid)
            {
                if (category.Id == 0)
                {
                    _db.Categories.Add(category);
                    _db.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }

                _db.Categories.Update(category);
                _db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            return View(category);
        }

        [HttpGet]
        public IActionResult Import()
        {
            return View();
        }
        
        
        [HttpPost]
        public async Task<IActionResult> Import(IFormFile file)
        {
            var list = new List<Category>();
            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                    var rowCount = worksheet.Dimension.Rows;
                    for (int row = 2; row <= rowCount; row++)
                    {
                        list.Add(new Category()
                        {
                            Name = worksheet.Cells[row, 1].Value.ToString().Trim(),
                            Description = worksheet.Cells[row, 2].Value.ToString().Trim()
                        });
                    }
                }
            }
            _db.Categories.AddRange(list);
            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var deleteId = _db.Categories.Find(id);
            _db.Categories.Remove(deleteId);
            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}