using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebBulky.DataAccess.Repository.IRepository;
using WebBulky.Utility;

namespace WebBulky.ViewComponents
{
    public class ShoppingCartViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;
        public ShoppingCartViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        //Fetching Cart count directly from DB with other proper way ...
        public async Task<IViewComponentResult> InvokeAsync()
        {
            //MS Tag helpers to get the current user logged in...
            var ClaimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = ClaimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim != null)
            {
                if (HttpContext.Session.GetInt32(SD.SessionCart) == null)
                {
                    HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value).Count());
                }
                return View(HttpContext.Session.GetInt32(SD.SessionCart));
            }
            else
            {
                HttpContext.Session.Clear();
                return View(0);
            }
        }
    }
}
