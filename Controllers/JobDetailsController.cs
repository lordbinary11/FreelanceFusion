using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FreelanceFusion.Models;
using System.Data.SqlClient;

namespace FreelanceFusion.Controllers;

public class JobDetailsController : Controller
{
   
    private readonly string connectionString = "Server=(localdb)\\MylocalDB;Database=FreelanceFusion;Trusted_Connection=True;MultipleActiveResultSets=True";
    public IActionResult Index(int JobID)
    {
        Job job = GetJobDetailsFromDatabase(JobID);

        if (job == null)
        {
            return NotFound();
        }

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
                    }
                }
            }
        }
        return job;
    }
}
