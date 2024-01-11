using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBulky.DataAccess.Data;
using WebBulky.DataAccess.Repository.IRepository;
using WebBulky.Models;
using WebBulky.Models.Models;

namespace WebBulky.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ApplicationDbContext _db;
        public ProductRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Product obj)
        {
            //_db.Products.Update(obj); //Before ImageUrl process Product was updating like this!

            //
            var objFromDB = _db.Products.FirstOrDefault(u=>u.Id==obj.Id);
            if (objFromDB != null)
            {
                objFromDB.Title = obj.Title;
                objFromDB.Description = obj.Description;
                objFromDB.Price = obj.Price;
                objFromDB.CategoryId = obj.CategoryId;
                objFromDB.ISBN = obj.ISBN;
                objFromDB.ListPrice = obj.ListPrice;
                objFromDB.Price50 = obj.Price50;
                objFromDB.Price100 = obj.Price100;
                objFromDB.Author = obj.Author;
                if (obj.ImageUrl != null)
                {
                    objFromDB.ImageUrl = obj.ImageUrl;
                }
            }
        }
    }
}
