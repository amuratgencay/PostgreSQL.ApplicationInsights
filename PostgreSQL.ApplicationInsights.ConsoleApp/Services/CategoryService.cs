using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using PostgreSQL.ApplicationInsights.ConsoleApp.Models;

namespace PostgreSQL.ApplicationInsights.ConsoleApp.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly SalesContext _salesContext;

        public CategoryService(SalesContext salesContext)
        {
            _salesContext = salesContext;
        }

        public Category GetCategoryById(int id) => _salesContext.Categories.FirstOrDefault(x => x.Id == id);

        public IEnumerable<Category> GetAll() => _salesContext.Categories;
    }
}