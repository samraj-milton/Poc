using Microsoft.AspNetCore.Mvc;
using PocMvcApp.Models;
using System.Text.Json;

namespace PocMvcApp.Controllers
{
    public class OrderController : Controller
    {
        private string OrderJsonPath => Path.Combine(Directory.GetCurrentDirectory(), "data", "orders.json");
        private string CustomerJsonPath => Path.Combine(Directory.GetCurrentDirectory(), "data", "customers.json");
        private string ProductJsonPath => Path.Combine(Directory.GetCurrentDirectory(), "data", "products.json");

        private List<Order> ReadOrders()
        {
            if (!System.IO.File.Exists(OrderJsonPath)) return new List<Order>();
            var json = System.IO.File.ReadAllText(OrderJsonPath);
            return JsonSerializer.Deserialize<List<Order>>(json) ?? new List<Order>();
        }

        private void WriteOrders(List<Order> orders)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(OrderJsonPath)!);
            var json = JsonSerializer.Serialize(orders, new JsonSerializerOptions { WriteIndented = true });
            System.IO.File.WriteAllText(OrderJsonPath, json);
        }

        private List<Customer> ReadCustomers()
        {
            if (!System.IO.File.Exists(CustomerJsonPath)) return new List<Customer>();
            var json = System.IO.File.ReadAllText(CustomerJsonPath);
            return JsonSerializer.Deserialize<List<Customer>>(json) ?? new List<Customer>();
        }

        private void WriteCustomers(List<Customer> customers)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(CustomerJsonPath)!);
            var json = JsonSerializer.Serialize(customers, new JsonSerializerOptions { WriteIndented = true });
            System.IO.File.WriteAllText(CustomerJsonPath, json);
        }

        private List<Product> ReadProducts()
        {
            if (!System.IO.File.Exists(ProductJsonPath)) return new List<Product>();
            var json = System.IO.File.ReadAllText(ProductJsonPath);
            return JsonSerializer.Deserialize<List<Product>>(json) ?? new List<Product>();
        }

        public IActionResult Index()
        {
            var orders = ReadOrders();
            var customers = ReadCustomers();
            var products = ReadProducts();

            // Populate customer and product details for orders
            foreach (var order in orders)
            {
                order.Customer = customers.FirstOrDefault(c => c.Id == order.CustomerId);
                foreach (var item in order.OrderItems)
                {
                    item.Product = products.FirstOrDefault(p => p.Id == item.ProductId);
                }
            }

            var vm = new OrderViewModel 
            { 
                Orders = orders,
                Customers = customers,
                Products = products
            };
            return View(vm);
        }

        public IActionResult Create()
        {
            var customers = ReadCustomers();
            var products = ReadProducts();
            var vm = new OrderViewModel 
            { 
                Order = new Order(),
                Customers = customers,
                Products = products
            };
            return View(vm);
        }

        [HttpPost]
        public IActionResult Create(Order order)
        {
            if (ModelState.IsValid)
            {
                var orders = ReadOrders();
                order.Id = orders.Count > 0 ? orders.Max(o => o.Id) + 1 : 1;
                order.OrderDate = DateTime.Now;
                
                // Assign IDs to order items
                for (int i = 0; i < order.OrderItems.Count; i++)
                {
                    order.OrderItems[i].Id = i + 1;
                    order.OrderItems[i].OrderId = order.Id;
                }

                orders.Add(order);
                WriteOrders(orders);
                return RedirectToAction("Index");
            }

            var customers = ReadCustomers();
            var products = ReadProducts();
            var vm = new OrderViewModel 
            { 
                Order = order,
                Customers = customers,
                Products = products
            };
            return View(vm);
        }

        public IActionResult Details(int id)
        {
            var orders = ReadOrders();
            var order = orders.FirstOrDefault(o => o.Id == id);
            if (order == null) return NotFound();

            var customers = ReadCustomers();
            var products = ReadProducts();

            // Populate customer and product details
            order.Customer = customers.FirstOrDefault(c => c.Id == order.CustomerId);
            foreach (var item in order.OrderItems)
            {
                item.Product = products.FirstOrDefault(p => p.Id == item.ProductId);
            }

            var vm = new OrderViewModel 
            { 
                Order = order,
                Customers = customers,
                Products = products
            };
            return View(vm);
        }

        public IActionResult Edit(int id)
        {
            var orders = ReadOrders();
            var order = orders.FirstOrDefault(o => o.Id == id);
            if (order == null) return NotFound();

            var customers = ReadCustomers();
            var products = ReadProducts();

            var vm = new OrderViewModel 
            { 
                Order = order,
                Customers = customers,
                Products = products
            };
            return View(vm);
        }

        [HttpPost]
        public IActionResult Edit(Order order)
        {
            if (ModelState.IsValid)
            {
                var orders = ReadOrders();
                var existing = orders.FirstOrDefault(o => o.Id == order.Id);
                if (existing == null) return NotFound();

                existing.CustomerId = order.CustomerId;
                existing.Status = order.Status;
                existing.Notes = order.Notes;
                existing.OrderItems = order.OrderItems;

                // Update order item IDs
                for (int i = 0; i < existing.OrderItems.Count; i++)
                {
                    existing.OrderItems[i].Id = i + 1;
                    existing.OrderItems[i].OrderId = existing.Id;
                }

                WriteOrders(orders);
                return RedirectToAction("Index");
            }

            var customers = ReadCustomers();
            var products = ReadProducts();
            var vm = new OrderViewModel 
            { 
                Order = order,
                Customers = customers,
                Products = products
            };
            return View(vm);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var orders = ReadOrders();
            var order = orders.FirstOrDefault(o => o.Id == id);
            if (order != null)
            {
                orders.Remove(order);
                WriteOrders(orders);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult UpdateStatus(int id, OrderStatus status)
        {
            var orders = ReadOrders();
            var order = orders.FirstOrDefault(o => o.Id == id);
            if (order != null)
            {
                order.Status = status;
                WriteOrders(orders);
            }
            return RedirectToAction("Index");
        }

        // Customer management methods
        public IActionResult Customers()
        {
            var customers = ReadCustomers();
            return View(customers);
        }

        [HttpPost]
        public IActionResult CreateCustomer(Customer customer)
        {
            if (ModelState.IsValid)
            {
                var customers = ReadCustomers();
                customer.Id = customers.Count > 0 ? customers.Max(c => c.Id) + 1 : 1;
                customer.CreatedDate = DateTime.Now;
                customers.Add(customer);
                WriteCustomers(customers);
            }
            return RedirectToAction("Customers");
        }

        [HttpPost]
        public IActionResult DeleteCustomer(int id)
        {
            var customers = ReadCustomers();
            var customer = customers.FirstOrDefault(c => c.Id == id);
            if (customer != null)
            {
                customers.Remove(customer);
                WriteCustomers(customers);
            }
            return RedirectToAction("Customers");
        }
    }
}