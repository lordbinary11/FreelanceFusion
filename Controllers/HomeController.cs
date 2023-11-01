using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FreelanceFusion.Models;
using System.Data.SqlClient;

namespace FreelanceFusion.Controllers;

public class HomeController : Controller
{
   private readonly string _connectionString = "Server=(localdb)\\MylocalDB;Database=FreelanceFusion;Trusted_Connection=True;MultipleActiveResultSets=True";
  
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult CategoryFilter(int categoryId, string searchQuery){
       List<Job> jobs = GetJobsFromDatabase(categoryId);
            if (!string.IsNullOrEmpty(searchQuery))
            {
                jobs = jobs.Where(j => j.Title.ToLower().Contains(searchQuery.ToLower())).ToList();
            }

            if (jobs.Count == 0)
            {
                ViewBag.Message = "No jobs found matching the search query.";
            }
            ViewBag.CategoryId = categoryId;

            return View(jobs);
    }

    private List<Job> GetJobsFromDatabase(int categoryId)
    {
        
        var jobs = new List<Job>(); // Assuming Job is your model class
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string query = "select j.jobID, j.title, j.budget, j.experience, j.Description, u.FirstName, u.LastName, c.CategoryName from job j join [user] u on j.UserID=u.UserID join Category c on j.CategoryID=c.CategoryID where j.CategoryID=@CategoryID";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@CategoryID", categoryId);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                // Create Job objects and add them to the jobs list
                // For example:
                var job = new Job
                            {
                                JobID = Convert.ToInt32(reader["jobID"]),
                                Title = reader["title"].ToString(),
                                Budget = reader["budget"].ToString(),
                                Experience = reader["Experience"].ToString(),
                                FirstName = reader["FirstName"].ToString(),
                                LastName = reader["LastName"].ToString(),
                                CategoryName = reader["CategoryName"].ToString(),
                                Description = reader["Description"].ToString(),
                                
                            };
                jobs.Add(job);
            }
            connection.Close();
        }

        
        // Pass the filtered jobs to the view
        return jobs;
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
