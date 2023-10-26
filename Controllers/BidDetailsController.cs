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
        return View(bid);
    }

    public IActionResult Indexx(int BidID)
    {
        Bid bid = GetBidDetailsFromDatabase(BidID);
         CurrentUser currentUser = GetCurrentLoggedInUser();
             

        if (bid == null)
        {
            return NotFound();
        }

        ViewBag.ShowButtons = bid.UserID != currentUser.UserID;
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
                        
                    }
                }
            }
        }
        return bid;
    }
}
