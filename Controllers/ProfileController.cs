using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Security.Claims;
using System.Threading.Tasks; // Added namespace for Task
using Microsoft.AspNetCore.Authentication; // Added namespace for Authentication
using FreelanceFusion.Models;

namespace FreelanceFusion.Controllers
{
    public class ProfileController : Controller
    {
        private int GetCurrentUserId()
        {
            var claimsIdentity = HttpContext.User.Identity as ClaimsIdentity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (int.TryParse(userId, out int id))
            {
                return id;
            }
            else
            {
                throw new Exception("Current user ID not found or invalid.");
            }
        }

        private readonly string connectionString = "Server=(localdb)\\MylocalDB;Database=FreelanceFusion;Trusted_Connection=True;MultipleActiveResultSets=True";

        public IActionResult Index()
        {
            var userprofile = GetUserDetailsFromDatabase();
            return View(userprofile);
        }

        public IActionResult EditProfile()
        {
            var userprofile = GetUserDetailsFromDatabase();
            return View(userprofile);
        }

        public IActionResult Save(UserProfile usr)
        {
            var userID = GetCurrentUserId();
            
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var command = new SqlCommand("UPDATE [User] SET  FirstName=@FirstName, LastName=@LastName, Email=@Email, Portfolio=@Portfolio, Bio=@Bio, Experience=@Experience, UserSkill=@UserSkill, Username=@Username where UserID=@userID", connection);
                command.Parameters.AddWithValue("@FirstName", usr.FirstName);
                command.Parameters.AddWithValue("@LastName", usr.LastName);
                command.Parameters.AddWithValue("@Email", usr.Email);
                command.Parameters.AddWithValue("@Portfolio", usr.Portfolio); // corrected parameter name
                command.Parameters.AddWithValue("@Bio", usr.Bio);
                command.Parameters.AddWithValue("@Experience", usr.Experience);
                command.Parameters.AddWithValue("@UserSkill", usr.UserSkill);
                command.Parameters.AddWithValue("@Username", usr.Username);
                command.Parameters.AddWithValue("@userID", userID);
                command.ExecuteNonQuery();
            }
            return RedirectToAction("Index", "Profile");
        }

        private UserProfile GetUserDetailsFromDatabase()
        {
            UserProfile usr = new UserProfile();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = "select * from [user] where userID=@UserID";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@UserID", GetCurrentUserId());
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            usr.UserID = (int)reader["UserID"];
                            usr.Bio = reader["Bio"].ToString();
                            usr.Experience = reader["Experience"].ToString();
                            usr.Portfolio = reader["Portfolio"].ToString();
                            usr.Password = reader["Password"].ToString();
                            usr.FirstName = reader["FirstName"].ToString();
                            usr.LastName = reader["LastName"].ToString();
                            usr.Username = reader["Username"].ToString();
                            usr.Email = reader["Email"].ToString();
                            usr.UserSkill = reader["UserSkill"].ToString();
                        }
                    }
                }
            }
            return usr;
        }

        public IActionResult ChangePassword()
        {
            return View();
        }

       public async Task<IActionResult> Auth(string OldPassword, string NewPassword)
{
    var userId = GetCurrentUserId();
    var connectionString = "Server=(localdb)\\MylocalDB;Database=FreelanceFusion;Trusted_Connection=True;MultipleActiveResultSets=True";

    using (var connection = new SqlConnection(connectionString))
    {
        connection.Open();
        var query = "SELECT Password FROM [User] WHERE UserID = @UserID";
        var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@UserID", userId);
        var dbPassword = command.ExecuteScalar() as string;

        if (dbPassword == OldPassword )
        {
            var updateQuery = "UPDATE [User] SET Password = @NewPassword WHERE UserID = @UserID";
            var updateCommand = new SqlCommand(updateQuery, connection);
            updateCommand.Parameters.AddWithValue("@NewPassword", NewPassword);
            updateCommand.Parameters.AddWithValue("@UserID", userId);
            updateCommand.ExecuteNonQuery();

            // Redirect to a success page
            return RedirectToAction("Index","Home");
        }
        else
        {
            if (dbPassword != OldPassword)
            {
                TempData["ErrorMessage"] = "Invalid old password.";
            }
            else if (NewPassword.Length<6)
            {
                TempData["ErrorMessage"] = "Minimum password lenghth is 6.";
            }

            return RedirectToAction("ChangePassword");
        }
    }
}


    








    public IActionResult Indexx(int userId)
{
    var userProfile = GetUserDetailsFromDatabase(userId);
    return View(userProfile);
}

private UserProfile GetUserDetailsFromDatabase(int userId)
{
    UserProfile usr = new UserProfile();
    using (SqlConnection connection = new SqlConnection(connectionString))
    {
        string sql = "select * from [user] where userID=@UserID";
        using (SqlCommand command = new SqlCommand(sql, connection))
        {
            command.Parameters.AddWithValue("@UserID", userId);
            connection.Open();
            using (SqlDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    usr.UserID = (int)reader["UserID"];
                    usr.Bio = reader["Bio"].ToString();
                    usr.Experience = reader["Experience"].ToString();
                    usr.Portfolio = reader["Portfolio"].ToString();
                    usr.Password = reader["Password"].ToString();
                    usr.FirstName = reader["FirstName"].ToString();
                    usr.LastName = reader["LastName"].ToString();
                    usr.Username = reader["Username"].ToString();
                    usr.Email = reader["Email"].ToString();
                    usr.UserSkill = reader["UserSkill"].ToString();
                }
            }
        }
    }
    return usr;
}



    }
}
