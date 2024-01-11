using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebBulky_RP.Data;
using WebBulky_RP.Models;

namespace WebBulky_RP.Pages.Categories
{
    // [BindProperties] It will bind all the properties.  //2nd way
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        [BindProperty] //It will Only bind the single (Category) prop. from the line below; //1st way - which further can be passes to OnPost methods.
        public Category Category { get; set; }
        public CreateModel(ApplicationDbContext db)
        {
            _db = db;
        }
        public void OnGet()
        {
        }
        public IActionResult OnPost()
        {
            _db.Categories.Add(Category);
            _db.SaveChanges();
            TempData["success"] = "Category added successfully.";
            return RedirectToPage("Index");
        }
    }
}
