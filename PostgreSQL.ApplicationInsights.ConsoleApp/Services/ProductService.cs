using System.Collections.Generic;
using System.Linq;
using PostgreSQL.ApplicationInsights.ConsoleApp.Models;

namespace PostgreSQL.ApplicationInsights.ConsoleApp.Services
{
    public class ProductService : IProductService
    {
        private readonly SalesContext _salesContext;

        public ProductService(SalesContext salesContext)
        {
            _salesContext = salesContext;
        }

        public Product GetProductById(int id) => _salesContext.Products.FirstOrDefault(x => x.Id == id);

        public IEnumerable<Product> GetAll() => _salesContext.Products;

        public IEnumerable<Product> GetProductsInCategory(int categoryId) =>
            _salesContext.Products.Where(x => x.CategoryId == categoryId);
    }
}