using System.Collections.Generic;
using System.Linq;
using PostgreSQL.ApplicationInsights.ConsoleApp.Models;

namespace PostgreSQL.ApplicationInsights.ConsoleApp.Migrations.Seeds
{
    public class ProductBuilder : ISeeder
    {
        private readonly SalesContext _salesContext;

        public ProductBuilder(SalesContext salesContext)
        {
            _salesContext = salesContext;
        }

        public void Seed()
        {
            var categories = _salesContext.Categories.ToDictionary(x => x.Name, y => y.Id);
            var categoryProducts = new Dictionary<int, List<(string name, decimal price)>>
            {
                {
                    categories["Mobile Phone"], new List<(string, decimal)>
                    {
                        ("APPLE iPhone SE 256GB", 14550),
                        ("SAMSUNG Galaxy Z Flip 256GB", 12000),
                        ("HUAWEI Y5 2019 16GB", 7500)
                    }
                },
                {
                    categories["Laptop"], new List<(string, decimal)>
                    {
                        ("HP Pavilion Gaming/1Y7D9EA/CORE İ5-10300H/8GB RAM/512GB SSD/GTX 1650-4GB/16.1\" Laptop",
                            25000),
                        ("ACER PT715-51 15.6\"/i7-7700HQ CPU/32Gb Bellek/512x2 SSD/ Full HD Win 10 Laptop Outlet 1187098",
                            20000),
                        ("APPLE MWP42TU/A MacBook Pro 13.3\" 2020/Core i5 2.0GHz/16GB/512GB SSD/Intel Iris Plus Laptop",
                            35000)
                    }
                },
                {
                    categories["TV"], new List<(string, decimal)>
                    {
                        ("SONY 65XH8077 65\"", 6000),
                        ("LG OLED65BX 65\"", 5000),
                        ("PHILIPS 43PFS6805 43\"", 4000)
                    }
                }
            };
            var productsInDb = _salesContext.Products.ToList();

            foreach (var (categoryId, products) in categoryProducts)
            {
                foreach (var (name, price) in products.Where(product =>
                    !productsInDb.Any(x => x.Name == product.name && x.CategoryId == categoryId)))
                {
                    _salesContext.Products.Add(new Product
                    {
                        CategoryId = categoryId,
                        Name = name,
                        UnitPrice = price
                    });
                }
            }


            _salesContext.SaveChanges();
        }
    }
}