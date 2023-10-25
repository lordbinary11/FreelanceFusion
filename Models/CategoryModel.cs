// CategoryModel.cs
namespace FreelanceFusion.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        
    }

    public class Job
{
    public int JobID { get; set; }
    public string Title { get; set; }
    public string Budget { get; set; }
    public string PostedDate { get; set; }
    public string Deadline { get; set; }
    public string Description { get; set; }
    public int CategoryID { get; set; }
    public string JobSkill {get;set;}
    public string Experience {get; set; }
    public string FirstName {get; set ;}
    public string LastName {get; set; }
    public string Email {get; set; }
}

public class UserCredentials
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }



public class UserSignup
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
}

}

