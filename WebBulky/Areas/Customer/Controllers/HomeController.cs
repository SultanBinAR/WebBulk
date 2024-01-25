using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using WebBulky.DataAccess.Repository.IRepository;
using WebBulky.Models;
using WebBulky.Models.Models;
using WebBulky.Utility;

namespace WebBulky.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> productList = _unitOfWork.Product.GetAll(includeProperties: "Category");
            return View(productList);
        }
        public IActionResult Details(int productId)
        {
            ShoppingCart cart = new()
            {
                Product = _unitOfWork.Product.Get(u => u.Id == productId, includeProperties: "Category"),
                Count = 1,
                ProductId = productId
            };
            return View(cart);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            //MS Tag helpers to get the current user logged in?
            var ClaimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = ClaimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            shoppingCart.ApplicationUserId = userId; //Populating ShoppingCart ApplicationUserId with Authorize User Id.

            //Exception handling for Duplicate entries in SCart. table
            ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.ApplicationUserId == userId &&
            u.ProductId == shoppingCart.ProductId);

            if (cartFromDb != null)
            {
                //Shopping cart exists | adding new count value into previous count value!!!
                cartFromDb.Count += shoppingCart.Count;
                _unitOfWork.ShoppingCart.Update(cartFromDb); //If we remove this line, still count will be updated-bcuz EFC is constantly tracking that activity & will update count on _unitOfWork.Save() invoked.
                _unitOfWork.Save();
            }
            else
            {
                //Add new cart record
                _unitOfWork.ShoppingCart.Add(shoppingCart);
                _unitOfWork.Save(); //1st It will save cart in DB, then we will be able to retreive from DB, right? for session!

                //Session Process | Adding Session in project
                //Getting Count of the current user has in shooping cart! [Remember for Bug]
                HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId).Count());
            }

            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }

        //Privacy Action Starts here
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}