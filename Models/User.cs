namespace MyMvc.Models
{
    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; } // In a real application, this should be hashed
    }
}