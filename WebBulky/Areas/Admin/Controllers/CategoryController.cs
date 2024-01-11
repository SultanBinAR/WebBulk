using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebBulky.DataAccess.Data;
using WebBulky.DataAccess.Repository.IRepository;
using WebBulky.Models;
using WebBulky.Utility;

namespace WebBulky.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)] //Can be implemented on each Index individually or can be added like this: "Globally".
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            List<Category> objCategoryList = _unitOfWork.Category.GetAll().ToList();
            return View(objCategoryList);
        }
        //By default it is a Get method.
        //CREATE METHOD
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost] //Data Annotation?
        public IActionResult Create(Category obj)
        {
            //Custom Validations Check | He remove these in video as they were not needed.
            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("Name", "Category Name & Display Order can't be same!");
            }
            if (obj.Name.ToLower() == "name")
            {
                ModelState.AddModelError("", "Name is reserved keyword, try another one!");
            }

            if (ModelState.IsValid) //Checks for all validations if true then it will add obj. to database.
            {
                _unitOfWork.Category.Add(obj);
                _unitOfWork.Save();
                TempData["success"] = "Category created successfully.";
                return RedirectToAction("Index");
            }
            return View();
        }
        //EDIT
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Category? categoryFromDb = _unitOfWork.Category.Get(u => u.Id == id); //Find() work on primary key
            //Other ways to get id fronDB
            //Category categoryFromDb1 = _db.Categories.FirstOrDefault(u=>u.Id==id);
            //Category categoryFromDb2 = _categoryRepo.Categories.Where(u=>u.Id == id).FirstOrDefault(); //Filteration
            if (categoryFromDb == null)
            {
                return NotFound();
            }

            return View(categoryFromDb);
        }
        [HttpPost] //Data Annotation?
        public IActionResult Edit(Category obj)
        {
            if (ModelState.IsValid) //Checks for all validations if true then it will add obj. to database.
            {
                _unitOfWork.Category.Update(obj);
                _unitOfWork.Save();
                TempData["success"] = "Category updated successfully.";
                return RedirectToAction("Index");
            }
            return View();
        }
        //DELETE
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Category? categoryFromDb = _unitOfWork.Category.Get(u => u.Id == id); //Find() work on primary key
            if (categoryFromDb == null)
            {
                return NotFound();
            }

            return View(categoryFromDb);
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            Category? obj = _unitOfWork.Category.Get(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            _unitOfWork.Category.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "Category deleted successfully.";
            return RedirectToAction("Index");
        }
    }
}
