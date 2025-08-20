using Microsoft.AspNetCore.Mvc;
using PocMvcApp.Models;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.Json;

namespace PocMvcApp.Controllers
{
    public class ProductController : Controller
    {
        private string JsonPath => Path.Combine(Directory.GetCurrentDirectory(), "data", "products.json");

        private List<Product> ReadProducts()
        {
            if (!System.IO.File.Exists(JsonPath)) return new List<Product>();
            var json = System.IO.File.ReadAllText(JsonPath);
            return JsonSerializer.Deserialize<List<Product>>(json) ?? new List<Product>();
        }

        private void WriteProducts(List<Product> products)
        {
            var json = JsonSerializer.Serialize(products);
            System.IO.File.WriteAllText(JsonPath, json);
        }

        public IActionResult Index()
        {
            var products = ReadProducts();
            var vm = new ProductViewModel { Products = products, Product = new Product() };
            return View("Product", vm);
        }

        public IActionResult Add()
        {
            var products = ReadProducts();
            var vm = new ProductViewModel { Products = products, Product = new Product() };
            return View("Product", vm);
        }

        [HttpPost]
        public IActionResult Add(Product product)
        {
            var products = ReadProducts();
            product.Id = products.Count > 0 ? products.Max(p => p.Id) + 1 : 1;
            products.Add(product);
            WriteProducts(products);
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var products = ReadProducts();
            var product = products.FirstOrDefault(p => p.Id == id) ?? new Product();
            var vm = new ProductViewModel { Products = products, Product = product };
            return View("Product", vm);
        }

        [HttpPost]
        public IActionResult Edit(Product product)
        {
            var products = ReadProducts();
            var existing = products.FirstOrDefault(p => p.Id == product.Id);
            if (existing == null) return NotFound();
            existing.Name = product.Name;
            existing.Price = product.Price;
            existing.Description = product.Description;
            existing.Category = product.Category;
            existing.Stock = product.Stock;
            WriteProducts(products);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var products = ReadProducts();
            var product = products.FirstOrDefault(p => p.Id == id);
            if (product != null)
            {
                products.Remove(product);
                WriteProducts(products);
            }
            return RedirectToAction("Index");
        }
    }
}
