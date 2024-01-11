using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebBulky_RP.Data;
using WebBulky_RP.Models;

namespace WebBulky_RP.Pages.Categories
{
    [BindProperties] //In Edit method, we will use [B.P.] here.
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        
        public Category? Category { get; set; }
        public EditModel(ApplicationDbContext db)
        {
            _db = db;
        }
        public void OnGet(int? id)
        {
            if (id != null || id != 0)
            {
                Category = _db.Categories.Find(id);
            }
        }
        public IActionResult OnPost()
        {
            if (ModelState.IsValid) //Checks for all validations if true then it will add obj. to database.
            {
                _db.Categories.Update(Category);
                _db.SaveChanges();
                TempData["success"] = "Category updated successfully.";
                return RedirectToPage("Index");
            }
            return Page();
        }
    }
}