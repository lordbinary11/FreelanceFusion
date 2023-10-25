using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FreelanceFusion.Models;
using FreelanceFusion.Services;
using System.Data.SqlClient;

namespace FreelanceFusion.Controllers;

public class PostJobController : Controller
{
    
    private readonly CategoryService _categoryService;

    public PostJobController(CategoryService categoryService)
    {
        
        _categoryService = categoryService;
    }

    public IActionResult PostJob()
    {
         List<Category> categories = _categoryService.GetCategories(); // Assuming GetCategories returns a list of Category objects
            ViewBag.Categories = categories; // Pass the categories to the view
            return View();
        
    }

     public IActionResult JobPostSuccess()
  {
    return View();
  }

  private int GetLoggedInUserId()
    {
        // Assuming you're using the User.Identity.Name property for the username
        var username = User.Identity.Name;

        string connectionString = "Server=(localdb)\\MylocalDB;Database=FreelanceFusion;Trusted_Connection=True;MultipleActiveResultSets=True";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            string query = "SELECT UserID FROM [User] WHERE Username = @Username";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Username", username);

            var result = command.ExecuteScalar();

            if (result != null && int.TryParse(result.ToString(), out int userId))
            {
                return userId;
            }
            else
            {
                // Handle the case when the user is not found or an error occurs
                return -1; // Return an appropriate default value or throw an exception
            }
        }
    }

public IActionResult Form1(Job j )
{

    var userId = GetLoggedInUserId();
    
    using (var connection = new SqlConnection("Server=(localdb)\\MylocalDB;Database=FreelanceFusion;Trusted_Connection=True;MultipleActiveResultSets=True"))
    {
        connection.Open();

            // Insert the job into the Job table
            var command = new SqlCommand("INSERT INTO Job (Title, Budget, Deadline, Description, CategoryID, JobSkill, UserID,Experience) VALUES (@Title, @Budget, @Deadline, @Description, @CategoryID, @JobSkill, @UserID, @Experience)", connection);
            command.Parameters.AddWithValue("@Title", j.Title);
            command.Parameters.AddWithValue("@Budget", j.Budget);
            command.Parameters.AddWithValue("@Deadline", j.Deadline);
            command.Parameters.AddWithValue("@Description", j.Description);
            command.Parameters.AddWithValue("@CategoryID", j.CategoryID);
            command.Parameters.AddWithValue("@JobSkill", j.JobSkill);
            command.Parameters.AddWithValue("@UserID", userId);
            command.Parameters.AddWithValue("@Experience", j.Experience);
            command.ExecuteNonQuery();

    }     
    return RedirectToAction("JobPostSuccess","PostJob");

}

 
}