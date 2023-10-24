using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FreelanceFusion.Models;
using System.Data.SqlClient;

namespace FreelanceFusion.Controllers;

public class SignUpController : Controller
{
    private readonly ILogger<SignUpController> _logger;

    public SignUpController(ILogger<SignUpController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult SignUpSuccess()
  {
    return View();
  }
    

    [HttpPost]
public IActionResult SignIn(UserSignup user)
{
    string connectionString = "Server=(localdb)\\MylocalDB;Database=FreelanceFusion;Trusted_Connection=True;MultipleActiveResultSets=True"; // Replace with your connection string

    using (SqlConnection connection = new SqlConnection(connectionString))
    {
        connection.Open();
            
             // Check if the passwords match
           if (user.Password != user.ConfirmPassword)
    {
        ModelState.AddModelError(string.Empty, "Passwords do not match.");
        // You can redirect to an error view or back to the signup page with an error message
        return View("Index");
    }

    // Check if the user already exists
    string checkQuery = "SELECT COUNT(*) FROM [User] WHERE Username = @Username";
    SqlCommand checkCommand = new SqlCommand(checkQuery, connection);
    checkCommand.Parameters.AddWithValue("@Username", user.Username);
    int existingCount = Convert.ToInt32(checkCommand.ExecuteScalar());

    if (existingCount > 0)
    {
        ModelState.AddModelError(string.Empty, "Username already exists.");
        // You can redirect to an error view or back to the signup page with an error message
        return View("Index");
    }

    // Insert the new user data
    string query = "INSERT INTO [User] (FirstName, LastName, Username, Email, Password) VALUES (@FirstName, @LastName, @Username, @Email, @Password)";
    SqlCommand command = new SqlCommand(query, connection);
    command.Parameters.AddWithValue("@FirstName", user.FirstName);
    command.Parameters.AddWithValue("@LastName", user.LastName);
    command.Parameters.AddWithValue("@Username", user.Username);
    command.Parameters.AddWithValue("@Email", user.Email);
    command.Parameters.AddWithValue("@Password", user.Password);

    command.ExecuteNonQuery();
    }

    // Redirect to a success page or perform other actions as needed
    return RedirectToAction("SignupSuccess", "SignUp");
}



    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
