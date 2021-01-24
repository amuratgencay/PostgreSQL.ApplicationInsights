using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PostgreSQL.ApplicationInsights.ConsoleApp.Models;
using PostgreSQL.ApplicationInsights.ConsoleApp.Services;
using static System.Console;

namespace PostgreSQL.ApplicationInsights.ConsoleApp.Views
{
    public class CategoryView : IView
    {
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;

        public CategoryView(ICategoryService categoryService, IProductService productService)
        {
            _categoryService = categoryService;
            _productService = productService;
        }

        public string Get(int categoryId)
        {
            
            var category = _categoryService.GetCategoryById(categoryId);
            Title = $"PostgreSQL Example - Category/{categoryId}";
            WriteLine("=======================================");
            WriteLine($"║ Products in {category.Name} Category".PadRight(38, ' ') + "║");
            WriteLine("=======================================");

            var products = _productService.GetProductsInCategory(categoryId).ToList();
            var i = 1;
            products.ForEach(product =>
            {
                WriteLine("".PadLeft(120, '='));
                WriteLine($"║ {i++}. {product.Name}".PadRight(119, ' ') + "║");
                WriteLine("".PadLeft(120, '='));
            });

            return CreateResponse(products);
        }

        private static string CreateResponse(IReadOnlyList<Product> products)
        {
            var response = "";
            do
            {
                var key = ReadKey(true);
                var productId = key.KeyChar - '0';
                if (productId > 0 && productId <= 9 && products.Count > (productId - 1))
                    response = $"Product/Get/{products[productId - 1].Id}";
                if (key.Key == ConsoleKey.Backspace)
                    response = nameof(ConsoleKey.Backspace);
            } while (string.IsNullOrEmpty(response));

            return response;
        }
    }
}