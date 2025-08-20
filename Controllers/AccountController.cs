using Microsoft.AspNetCore.Mvc;

namespace PocMvcApp.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }

        private string UserJsonPath => Path.Combine(Directory.GetCurrentDirectory(), "data", "users.json");

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            // Optionally clear cookies if used
            return RedirectToAction("Login");
        }

        private class User
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }

        private List<User> ReadUsers()
        {
            if (!System.IO.File.Exists(UserJsonPath)) return new List<User>();
            var json = System.IO.File.ReadAllText(UserJsonPath);
            return System.Text.Json.JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var users = ReadUsers();
            var user = users.FirstOrDefault(u => u.Username == username && u.Password == password);
            if (user != null)
            {
                // Set a session or cookie here for real apps
                return RedirectToAction("Index", "Product");
            }
            ViewBag.Error = "Invalid credentials";
            return View();
        }
    }
}
