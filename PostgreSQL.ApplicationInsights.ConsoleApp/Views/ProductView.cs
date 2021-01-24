using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PostgreSQL.ApplicationInsights.ConsoleApp.Services;
using static System.Console;

namespace PostgreSQL.ApplicationInsights.ConsoleApp.Views
{
    public class ProductView : IView
    {
        private readonly IProductService _productService;

        public ProductView(IProductService productService)
        {
            _productService = productService;
        }

        public string Get(int productId)
        {
            var product = _productService.GetProductById(productId);
            Title = $"PostgreSQL Example - Product/{productId}";

            WriteLine("".PadLeft(120, '='));
            var line1 = $"║ Product: {product.Name,-25}";
            var line2 = $"║ Price  : {product.UnitPrice:C}";
            WriteLine(line1.PadRight(119,' ')+"║");
            WriteLine(line2.PadRight(119,' ')+"║");
            WriteLine("".PadLeft(120, '='));


            return CreateResponse();
        }

        private static string CreateResponse()
        {
            var response = "";
            do
            {
                var key = ReadKey(true);
                if (key.Key == ConsoleKey.Backspace)
                    response = nameof(ConsoleKey.Backspace);
            } while (string.IsNullOrEmpty(response));

            return response;
        }
    }
}