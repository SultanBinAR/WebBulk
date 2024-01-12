using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using System.Collections.Generic;
using System.Data;
using WebBulky.DataAccess.Data;
using WebBulky.DataAccess.Repository.IRepository;
using WebBulky.Models;
using WebBulky.Models.Models;
using WebBulky.Models.Models.ViewModels;
using WebBulky.Utility;

namespace WebBulky.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        //Injecting IWebHost Env. 
        private readonly IWebHostEnvironment _webHostEnvionment;

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvionment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvionment = webHostEnvionment;
        }

        public IActionResult Index()
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties:"Category").ToList();
            return View(objProductList);
        }
        //CREATE METHOD
        public IActionResult Upsert(int? id)
        {
            //ViewBag.CategoryList = CategoryList; //ViewData["CategoryList"] = CategoryList;

            ProductVM productVM = new()
            {
                CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                Product = new Product()
            };
            if (id == null || id == 0)
            {
                //Create
                return View(productVM);
            }
            else
            {
                //Update
                productVM.Product = _unitOfWork.Product.Get(u => u.Id == id );
                return View(productVM);
            }
        }
        //UPSERT METHOD
        [HttpPost]
        public IActionResult Upsert(ProductVM productVM, IFormFile? file)
        {
            /*///Custom Validations Check | He remove these in video as they were not needed.
            //if (obj.Title == obj.Description.ToString())
            //{
            //    ModelState.AddModelError("Name", "Product Name & Display Order can't be same!");
            //}
            //if (obj.Title.ToLower() == "title")
            //{
            //    ModelState.AddModelError("", "Name is reserved keyword, try another one!");
            //}*/

            if (ModelState.IsValid) //Checks for all validations if true then it will add obj. to database.
            {
                //Media Handling
                string wwwRootPath = _webHostEnvionment.WebRootPath;
                if (file != null)
                {
                    //fileName creation - Step 01
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"images\products");

                    if (!string.IsNullOrEmpty(productVM.Product.ImageUrl))
                    {
                        //deleting the old/existing image
                        var oldImagePath =
                            Path.Combine(wwwRootPath, productVM.Product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    //file uploading to root folder - Step 02 (for create method)
                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName),FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    //& Finallyy saving to ImagesUrl prop. in model - Step 03(Final)
                    productVM.Product.ImageUrl = @"\images\products\" + fileName;
                }


                //Product Saving
                if (productVM.Product.Id == 0)
                {
                    _unitOfWork.Product.Add(productVM.Product);
                    TempData["success"] = "Product created successfully.";
                }
                else
                {
                    _unitOfWork.Product.Update(productVM.Product);
                    TempData["success"] = "Product updated successfully.";
                }
                _unitOfWork.Save();
                return RedirectToAction("Index");
            }
            return View();
        }
        //EDIT METHOD - [Functional untill Upsert() was not define & called.]
        /*public IActionResult Edit(int? id)
        //{
        //    if (id == null || id == 0)
        //    {
        //        return NotFound();
        //    }
        //    Product? ProductFromDb = _unitOfWork.Product.Get(u => u.Id == id); //Find() work on primary key
        //    //Other ways to get id fronDB
        //    //Product ProductFromDb1 = _db.Categories.FirstOrDefault(u=>u.Id==id);
        //    //Product ProductFromDb2 = _ProductRepo.Categories.Where(u=>u.Id == id).FirstOrDefault(); //Filteration
        //    if (ProductFromDb == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(ProductFromDb);
        //}
        //[HttpPost] //Data Annotation?
        //public IActionResult Edit(Product obj)
        //{
        //    if (ModelState.IsValid) //Checks for all validations if true then it will add obj. to database.
        //    {
        //        _unitOfWork.Product.Update(obj);
        //        _unitOfWork.Save();
        //        TempData["success"] = "Product updated successfully.";
        //        return RedirectToAction("Index");
        //    }
        //    return View();
        //}*/


        //DELETE METHOD - [Functional untill API Calling not configured.]
        /*public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Product? ProductFromDb = _unitOfWork.Product.Get(u => u.Id == id); //Find() work on primary key
            if (ProductFromDb == null)
            {
                return NotFound();
            }

            return View(ProductFromDb);
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            Product? obj = _unitOfWork.Product.Get(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            _unitOfWork.Product.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "Product deleted successfully.";
            return RedirectToAction("Index");
        }*/

        #region API CALLs (Basic built-in API EndPoint/Function/Call to get any data from obj. in Json form.)

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return Json(new { data = objProductList });
        }

        //DELETE METHOD [2nd] (No need DA like: [HttpDelete] | It worked after i deleted the View for the above method Delete() .)
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var productToBeDeleted = _unitOfWork.Product.Get(u => u.Id == id);
            if (productToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            //deleting the old/existing image
            var oldImagePath =
                Path.Combine(_webHostEnvionment.WebRootPath,
                productToBeDeleted.ImageUrl.TrimStart('\\'));
            
            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }
            _unitOfWork.Product.Remove(productToBeDeleted);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Delete successfull" });
        }
        #endregion
    }
}
