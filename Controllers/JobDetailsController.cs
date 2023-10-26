using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Security.Claims;
using FreelanceFusion.Models;

namespace FreelanceFusion.Controllers;

public class JobDetailsController : Controller
{
   
    private CurrentUser GetCurrentLoggedInUser()
        {
            var claimsIdentity = HttpContext.User.Identity as ClaimsIdentity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (int.TryParse(userId, out int id))
            {
                return new CurrentUser { UserID = id };
            }
            else
            {
                // Handle the case where the user ID is not available or not in the expected format
                // For example, you can return a default user ID or throw an exception
                throw new Exception("Current user ID not found or invalid.");
            }
       }


    private readonly string connectionString = "Server=(localdb)\\MylocalDB;Database=FreelanceFusion;Trusted_Connection=True;MultipleActiveResultSets=True";
    public IActionResult Index(int JobID)
    {
        Job job = GetJobDetailsFromDatabase(JobID);
        CurrentUser currentUser = GetCurrentLoggedInUser();

        if (job == null)
        {
            return NotFound();
        }
        
        ViewBag.ShowButtons = job.UserID != currentUser.UserID;
        return View(job);
    }

    public IActionResult NotFound()
{
    return View();
}


    private Job GetJobDetailsFromDatabase(int JobID)
    {
        Job job = new Job();
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            string sql ="select * from job j join [user] u on j.UserID=u.UserID where j.JobID=@JobID";
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@JobID", JobID);
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        job.JobID = (int)reader["JobId"];
                        job.Title = reader["Title"].ToString();
                        job.Description = reader["Description"].ToString();
                        job.Budget = reader["Budget"].ToString();
                        job.PostedDate = reader["PostedDate"].ToString();
                        job.Deadline = reader["Deadline"].ToString();
                        job.JobSkill = reader["JobSkill"].ToString();
                        job.Experience = reader["Experience"].ToString();
                        job.FirstName = reader["FirstName"].ToString();
                        job.LastName = reader["LastName"].ToString();
                        job.Email = reader["Email"].ToString();
                        job.UserID = (int)reader["UserID"];
                    }
                }
            }
        }
        return job;
    }
}
