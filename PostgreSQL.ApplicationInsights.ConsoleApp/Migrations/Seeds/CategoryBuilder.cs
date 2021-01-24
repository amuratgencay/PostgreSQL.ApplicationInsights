using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PostgreSQL.ApplicationInsights.ConsoleApp.Models;

namespace PostgreSQL.ApplicationInsights.ConsoleApp.Migrations.Seeds
{
    public class CategoryBuilder : ISeeder
    {
        private readonly SalesContext _salesContext;

        public CategoryBuilder(SalesContext salesContext)
        {
            _salesContext = salesContext;
        }

        public void Seed()
        {
            var categoryNames = new[] {"Mobile Phone", "Laptop", "TV"};
            var categoriesInDb = _salesContext.Categories.Select(x => x.Name).ToList();
            var categories = categoryNames.Except(categoriesInDb)
                .Select(categoryName => new Category {Name = categoryName});

            if (categories.Any())
                _salesContext.Categories.AddRange(categories);

            _salesContext.SaveChanges();
        }
    }
}