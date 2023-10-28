using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Security.Claims;
using FreelanceFusion.Models;

namespace FreelanceFusion.Controllers
{
    public class DashboardController : Controller
    {
        private readonly string _connectionString = "Server=(localdb)\\MylocalDB;Database=FreelanceFusion;Trusted_Connection=True;MultipleActiveResultSets=True";

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

        public IActionResult ClientView()
        {
            var jobs = GetJobsForCurrentUser();
            var bids = GetBidsForCurrentUser();
            ViewBag.ShowButtons = true;

            return View((jobs, bids));
        }

        public IActionResult FreelancerView()
        {
            var bids = GetBidsMadeByCurrentUser();
            ViewBag.ShowButtons = false;
            return View(bids);
        }


        private List<Job> GetJobsForCurrentUser()
        {
            List<Job> jobs = new List<Job>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string sql = "SELECT * FROM job j  JOIN [user] u  ON j.UserID = u.UserID where j.UserID = @UserId";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@UserId", GetCurrentUserId());
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Job job = new Job
                            {
                                JobID = (int)reader["JobId"],
                                Title = reader["Title"].ToString(),
                                Budget = reader["Budget"].ToString(),
                                PostedDate = reader["PostedDate"].ToString(),
                                Deadline = reader["Deadline"].ToString(),
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

        private List<Bid> GetBidsForCurrentUser()
        {
            List<Bid> bids = new List<Bid>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string sql = "SELECT b.BidID, b.IsAWarded, b.JobID, b.BidAmount, b.BidMessage, b.BidDate, j.Title, u.FirstName, u.LastName FROM Bid b JOIN Job j ON b.JobID = j.JobID JOIN [User] u ON b.UserID = u.UserID where j.UserID=@UserId";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@UserId", GetCurrentUserId());
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Bid bid = new Bid
                            {
                                BidID = (int)reader["BidID"],
                                BidStatus = reader["IsAwarded"].ToString(),
                                BidAmount = reader["BidAmount"].ToString(),
                                BidMessage = reader["BidMessage"].ToString(),
                                BidDate = reader["BidDate"].ToString(),
                                JobTitle = reader["Title"].ToString(),
                                FirstName = reader["FirstName"].ToString(),
                                LastName = reader["LastName"].ToString(),
                                JobID = reader["JobID"].ToString(),
                            };
                            bids.Add(bid);
                        }
                    }
                }
            }
            return bids;
        }


        private List<Bid> GetBidsMadeByCurrentUser()
        {
            List<Bid> bids = new List<Bid>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string sql = "SELECT b.BidID, b.IsAWarded, b.JobID, b.BidAmount, b.BidMessage, b.BidDate, j.Title, u.FirstName, u.LastName FROM Bid b JOIN Job j ON b.JobID = j.JobID JOIN [User] u ON b.UserID = u.UserID where b.UserID=@UserId";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@UserId", GetCurrentUserId());
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Bid bid = new Bid
                            {
                                BidID = (int)reader["BidID"],
                                BidStatus = reader["IsAwarded"].ToString(),
                                BidAmount = reader["BidAmount"].ToString(),
                                BidMessage = reader["BidMessage"].ToString(),
                                BidDate = reader["BidDate"].ToString(),
                                JobTitle = reader["Title"].ToString(),
                                FirstName = reader["FirstName"].ToString(),
                                LastName = reader["LastName"].ToString(),
                                JobID = reader["JobID"].ToString(),
                            };
                            bids.Add(bid);
                        }
                    }
                }
            }
            return bids;
        }

        public IActionResult DeleteJob(int jobId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string sql = "DELETE FROM job WHERE JobID = @JobID";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@JobID", jobId);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error");
            }
            return RedirectToAction("FreelancerView");
        }
    }
}
