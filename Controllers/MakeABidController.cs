using Microsoft.AspNetCore.Mvc;
using System;
using System.Data.SqlClient;
using System.Security.Claims;
using FreelanceFusion.Models;

namespace FreelanceFusion.Controllers
{
    public class MakeABidController : Controller
    {
        private readonly string _connectionString = "Server=(localdb)\\MylocalDB;Database=FreelanceFusion;Trusted_Connection=True;MultipleActiveResultSets=True";

        // Method to get the current user's ID
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

        
        // Method to check the number of bids made by the user for a specific job
private int GetBidCountForJob(int userId, int JobID)
{
    int bidCount = 0;
    using (SqlConnection connection = new SqlConnection(_connectionString))
    {
        string sql = "SELECT COUNT(*) FROM Bid WHERE UserID = @UserId AND JobID = @JobId";
        using (SqlCommand command = new SqlCommand(sql, connection))
        {
            command.Parameters.AddWithValue("@UserId", userId);
            command.Parameters.AddWithValue("@JobId", JobID);
            connection.Open();
            bidCount = (int)command.ExecuteScalar();
        }
    }
    return bidCount;
}



        public IActionResult MakeABid(int JobID)
        {
            var bid = new Bid
            {
                JobID = JobID.ToString()
            };

            return View(bid);
        }

        public IActionResult BidSuccess()
        {
            return View();
        }

        public IActionResult BidLimitExceeded()
        {
            return View();
        }

        [HttpPost]
        public IActionResult MakeABid(int JobID, Bid bid)
        {
            var userId = GetCurrentUserId();
            var Status = "0";
            int bidCount = GetBidCountForJob(userId, JobID);

              if (bidCount > 1)
    {
        // Prompt the user and deny the making of the bid
        // You can return an error message or redirect the user to an error page
        return RedirectToAction("BidLimitExceeded");
    }
         else{
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string sql = "INSERT INTO Bid (UserID, JobID, BidAmount, BidMessage, IsAwarded) VALUES (@UserId, @JobId, @BidAmount, @BidMessage, @Status)";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    command.Parameters.AddWithValue("@JobId", JobID);
                    command.Parameters.AddWithValue("@BidAmount", bid.BidAmount);
                    command.Parameters.AddWithValue("@BidMessage", bid.BidMessage);
                    command.Parameters.AddWithValue("@Status", Status);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }

            return RedirectToAction("BidSuccess", "MakeABid");
         }    
        }
    }
}
