using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using FreelanceFusion.Models; // Or your namespace for the UserCredentials model


namespace FreelanceFusion.Controllers
{
    public class LoginController : Controller
    {
        private readonly ILogger<LoginController> _logger;

        // public bool IsLoggedIn { get; set; }

        public LoginController(ILogger<LoginController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
       
        public async Task<IActionResult> Authentication(UserCredentials credentials)
        {
            string connectionString = "Server=(localdb)\\MylocalDB;Database=FreelanceFusion;Trusted_Connection=True;MultipleActiveResultSets=True"; // Replace with your connection string

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT UserID FROM [User] WHERE Username = @Username AND Password = @Password";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Username", credentials.Username);
                command.Parameters.AddWithValue("@Password", credentials.Password);

                int UserID = Convert.ToInt32(command.ExecuteScalar());

                if (UserID > 0)
            {
                
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, credentials.Username),
                 new Claim(ClaimTypes.NameIdentifier, UserID.ToString()),
                 new Claim("Password", credentials.Password)
                
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));

            return RedirectToAction("Index", "Home");
        }
                else
                {
                    // Authentication failed, return the login view with an error message
                     ModelState.AddModelError(string.Empty, "Invalid username or password.");
                    return View("Index");
                }
            }
        }

       
    }
}
