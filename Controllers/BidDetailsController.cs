using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Security.Claims;
using FreelanceFusion.Models;

namespace FreelanceFusion.Controllers;

public class BidDetailsController : Controller
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
    public IActionResult Index(int BidID)
    {
        Bid bid = GetBidDetailsFromDatabase(BidID);
    
        if (bid == null)
        {
            return NotFound();
        }

        // int jobID = int.Parse(JobID);
          
        //   bool isBidAwarded = IsBidAwarded(jobID, BidID);
        //   ViewBag.IsBidAwarded = isBidAwarded;
        return View(bid);
    }

    public IActionResult Indexx(int BidID )
    {
        Bid bid = GetBidDetailsFromDatabase(BidID);
         CurrentUser currentUser = GetCurrentLoggedInUser();
             

        if (bid == null)
        {
            return NotFound();
        }

       

      
        return View(bid);
    }

    public IActionResult NotFound()
{
    return View();
}


    public Bid GetBidDetailsFromDatabase(int BidID)
    {
        Bid bid = new Bid();
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            string sql ="select * from bid join job on bid.JobID = job.JobID where BidID=@BidID";
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@BidID", BidID);
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        bid.BidID = (int)reader["BidId"];
                        bid.BidAmount = reader["BidAmount"].ToString();
                        bid.BidMessage = reader["bidMessage"].ToString();
                        bid.BidDate = reader["bidDate"].ToString();
                        bid.JobTitle = reader["Title"].ToString();
                        bid.UserID =(int)reader["UserID"];
                        bid.JobID = reader ["JobID"].ToString();
                        bid.BidStatus = reader["IsAwarded"].ToString();
                        
                    }
                }
            }
        }
        return bid;
    }

       [HttpPost]
public IActionResult AcceptBid(int bidId, int jobId)
{
    // Update the bid status in the database to 1 for the provided bidId
    using (SqlConnection connection = new SqlConnection(connectionString))
    {
        connection.Open();
        string updateQuery = "UPDATE Bid SET IsAwarded = 1 WHERE BidID = @bidId";
        SqlCommand command = new SqlCommand(updateQuery, connection);
        command.Parameters.AddWithValue("@bidId", bidId);
        command.ExecuteNonQuery();
    }

    // Notify other bids with the same jobId
    using (SqlConnection connection = new SqlConnection(connectionString))
    {
        connection.Open();
        string notifyQuery = "UPDATE Bid SET IsAwarded = 2 WHERE JobID = @jobId AND IsAwarded = 0 AND BidID != @bidId";
        SqlCommand command = new SqlCommand(notifyQuery, connection);
        command.Parameters.AddWithValue("@jobId", jobId);
        command.Parameters.AddWithValue("@bidId", bidId);
        command.ExecuteNonQuery();
    }


    // // Set TempData message based on the bid status
    // if (IsBidAwarded(bidId, jobId))
    // {
    //     TempData["BidMessage"] = "Bid Awarded";
    // }
   

    return RedirectToAction("Index", new { bidId });
}
    

// private bool IsBidAwarded(int bidId, int jobId)
// {
//     using (SqlConnection connection = new SqlConnection(connectionString))
//     {
//         connection.Open();
//         string query = "SELECT IsAwarded FROM Bid WHERE BidID = @bidId AND JobID = @jobId";
//         SqlCommand command = new SqlCommand(query, connection);
//         command.Parameters.AddWithValue("@bidId", bidId);
//         command.Parameters.AddWithValue("@jobId", jobId);

//         using (SqlDataReader reader = command.ExecuteReader())
//         {
//             if (reader.Read())
//             {
//                 return Convert.ToBoolean(reader["IsAwarded"]);
//             }
//             else{
//                 return false; // if no records were found
//             }
//         }
//     }    



// }
}


