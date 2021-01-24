using System.Collections.Generic;
using PostgreSQL.ApplicationInsights.ConsoleApp.Models;

namespace PostgreSQL.ApplicationInsights.ConsoleApp.Services
{
    public interface IProductService
    {
        Product GetProductById(int id);
        IEnumerable<Product> GetAll();
        IEnumerable<Product> GetProductsInCategory(int categoryId);
    }
}