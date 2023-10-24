using System.Collections.Generic;
using System.Data.SqlClient;
using FreelanceFusion.Models;

namespace FreelanceFusion.Services
{
    public class CategoryService
   {
        public string _connectionString="Server=(localdb)\\MylocalDB;Database=FreelanceFusion;Trusted_Connection=True;MultipleActiveResultSets=True";

        public CategoryService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Category> GetCategories()
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = new SqlCommand("SELECT CategoryName, CategoryId FROM CATEGORY", connection);
            connection.Open();

            SqlDataReader reader = command.ExecuteReader();

            List<Category> categories = new List<Category>();
            while (reader.Read())
            {
                Category category = new Category();
                category.CategoryName = reader["CategoryName"].ToString();
                category.CategoryId = (int)reader["CategoryId"];
                categories.Add(category);
            }

            return categories;
        }
    }

    public class AuthenticationHelper
{
    public static bool IsAuthenticated(UserCredentials credentials)

    {

        
        // Implement your custom authentication logic here

        // For example, you could check for a valid authentication cookie
        // or check if the user is logged in to a database

        // return false if the user is not authenticated
        // return true if the user is authenticated

        // Replace the below example with your custom authentication logic

        // For instance, check if the user exists in the database


        string connectionString = "Server=(localdb)\\MylocalDB;Database=FreelanceFusion;Trusted_Connection=True;MultipleActiveResultSets=True"; // Replace with your connection string

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            string query = "SELECT COUNT(*) FROM [User] WHERE Username = @Username AND Password = @Password";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Username", credentials.Username);
            command.Parameters.AddWithValue("@Password", credentials.Password);

            int count = Convert.ToInt32(command.ExecuteScalar());
            
            
            if (count > 0)
            {
                // Authentication successful
                return true;
            }
            else if( credentials == null)
            {
                // Authentication failed
                return false;
            }
            else{
                return false;
            }
        }
    }
}

}
