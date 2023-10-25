using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FreelanceFusion.Models;
using System.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FreelanceFusion.Controllers
{
    public class JoblistingController : Controller
    {
        private readonly string connectionString = "Server=(localdb)\\MylocalDB;Database=FreelanceFusion;Trusted_Connection=True;MultipleActiveResultSets=True";

        public IActionResult Index(string searchQuery)
        {
            List<Job> jobs = GetJobsFromDatabase();
            if (!string.IsNullOrEmpty(searchQuery))
            {
                jobs = jobs.Where(j => j.Title.ToLower().Contains(searchQuery.ToLower())).ToList();
            }

            if (jobs.Count == 0)
            {
                ViewBag.Message = "No jobs found matching the search query.";
            }
            return View(jobs);
        }

        private List<Job> GetJobsFromDatabase()
        {
            List<Job> jobs = new List<Job>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = "select j.jobID, j.title, j.budget, j.experience, u.FirstName, u.LastName from job j join [user] u on j.UserID=u.UserID";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Job job = new Job
                            {
                                JobID = Convert.ToInt32(reader["jobID"]),
                                Title = reader["title"].ToString(),
                                Budget = reader["budget"].ToString(),
                                Experience = reader["Experience"].ToString(),
                                FirstName = reader["FirstName"].ToString(),
                                LastName = reader["LastName"].ToString()
                            };
                            jobs.Add(job);
                        }
                    }
                }
            }
            return jobs;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
