using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace PocMvcApp.Models
{
    public enum OrderStatus
    {
        Pending = 1,
        Processing = 2,
        Shipped = 3,
        Delivered = 4,
        Cancelled = 5
    }

    public class Order
    {
        public int Id { get; set; }
        
        [Required]
        public int CustomerId { get; set; }
        
        public Customer? Customer { get; set; }
        
        public DateTime OrderDate { get; set; } = DateTime.Now;
        
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        
        public decimal TotalAmount => OrderItems?.Sum(item => item.TotalPrice) ?? 0;
        
        public string Notes { get; set; } = string.Empty;
    }

    public class OrderItem
    {
        public int Id { get; set; }
        
        public int OrderId { get; set; }
        
        [Required]
        public int ProductId { get; set; }
        
        public Product? Product { get; set; }
        
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }
        
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Unit price must be greater than 0")]
        public decimal UnitPrice { get; set; }
        
        public decimal TotalPrice => Quantity * UnitPrice;
    }

    public class OrderViewModel
    {
        public IEnumerable<Order> Orders { get; set; } = new List<Order>();
        public Order Order { get; set; } = new Order();
        public IEnumerable<Customer> Customers { get; set; } = new List<Customer>();
        public IEnumerable<Product> Products { get; set; } = new List<Product>();
    }
}