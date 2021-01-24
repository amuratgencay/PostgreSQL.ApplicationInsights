using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PostgreSQL.ApplicationInsights.ConsoleApp.Services;
using static System.Console;

namespace PostgreSQL.ApplicationInsights.ConsoleApp.Views
{
    public class IndexView : IView
    {
        private readonly ICategoryService _categoryService;

        public IndexView(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public string Get()
        {
            Title = "PostgreSQL Example - Index";

            WriteLine("=======================================");
            WriteLine("║              Categories             ║");
            WriteLine("=======================================");

            _categoryService.GetAll().ToList().ForEach(category =>
            {
                WriteLine("=======================================");
                WriteLine($"║ {category.Id}. {category.Name,-15} Product Count: {category.Products.Count} ║");
                WriteLine("=======================================");
            });

            return CreateResponse();
        }

        private static string CreateResponse()
        {
            var response = "";
            do
            {
                var key = ReadKey(true);
                var categoryId = key.KeyChar - '0';
                if (categoryId > 0 && categoryId <= 9)
                    response = $"Category/Get/{categoryId}";
                if (key.Key == ConsoleKey.Escape)
                    response = nameof(ConsoleKey.Escape);
            } while (string.IsNullOrEmpty(response));

            return response;
        }
    }
}